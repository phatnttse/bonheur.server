using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Payment;
using Bonheur.Services.DTOs.Payment.PayOs;
using Bonheur.Services.DTOs.Storage;
using Bonheur.Services.Email;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using PdfSharp.Pdf;


namespace Bonheur.Services
{
    public class PaymentService : IPaymentService
    {

        private readonly string _payOsPaymentSuccessUrl;
        private readonly string _payOsPaymentCancelUrl;
        private readonly PayOS _payOS;
        private readonly ISubscriptionPackageRepository _subscriptionPackageRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IInvoiceService _invoiceService;
        private readonly IStorageService _storageService;
        private readonly IEmailSender _emailSender;

        public PaymentService(
            PayOS payOS, 
            ISubscriptionPackageRepository subscriptionPackageRepository, 
            IInvoiceRepository invoiceRepository, IOrderRepository orderRepository, 
            IOrderDetailRepository orderDetailRepository, 
            IUserAccountRepository userAccountRepository,
            ISupplierRepository supplierRepository,
            IInvoiceService invoiceService,
            IStorageService storageService,
            IEmailSender emailSender
        )
        {
            _payOS = payOS;
            _subscriptionPackageRepository = subscriptionPackageRepository;
            _payOsPaymentSuccessUrl = Environment.GetEnvironmentVariable("PAYOS_PAYMENT_SUCCESS_URL") ?? throw new Exception("Environtment string 'PAYOS_PAYMENT_SUCCESS_URL' not found");
            _payOsPaymentCancelUrl = Environment.GetEnvironmentVariable("PAYOS_PAYMENT_CANCEL_URL") ?? throw new Exception("Environtment string 'PAYOS_PAYMENT_CANCEL_URL' not found");
            _invoiceRepository = invoiceRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _userAccountRepository = userAccountRepository;
            _supplierRepository = supplierRepository;
            _invoiceService = invoiceService;
            _storageService = storageService;
            _emailSender = emailSender;
        }

        public async Task<PaymentResponse> PayOsTransferHandler(WebhookType body)
        {
            try
            {
                WebhookData data = _payOS.verifyPaymentWebhookData(body);

                //if (data.description == "VQRIO123") return new PaymentResponse(0, "Ok", null); // confirm webhook

                if (data.code == "00")
                {
                    #region validate nullable values                  

                    Order? order = await _orderRepository.GetOrderByCodeAsync((int)data.orderCode);

                    if (order == null) throw new ApiException("Order not found", System.Net.HttpStatusCode.NotFound);

                    ApplicationUser? account = await _userAccountRepository.GetUserByIdAsync(order.UserId!);

                    if (account == null) throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                    Supplier? supplier = await _supplierRepository.GetSupplierByIdAsync((int)order.SupplierId!, false);

                    if (supplier == null) throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                    #endregion

                    #region Update order status                   
                    if (order.Status == OrderStatus.Active) throw new ApiException("Order already active", System.Net.HttpStatusCode.BadRequest);

                        if (order.PaymentStatus == PaymentStatus.Success) throw new ApiException("Payment already successful", System.Net.HttpStatusCode.BadRequest);

                        if (order.SupplierId != supplier.Id) throw new ApiException("You are not the owner of this order", System.Net.HttpStatusCode.Unauthorized);

                        order.Status = OrderStatus.Active;
                        order.PaymentStatus = PaymentStatus.Success;
                        #endregion

                        #region Update subscription package for supplier
                        int spId = order.OrderDetails?.ToList()[0].SubscriptionPackageId ?? throw new ApiException("Subscription package id not found", System.Net.HttpStatusCode.NotFound);

                        var subscriptionPackage = await _subscriptionPackageRepository.GetSubscriptionPackageByIdAsync(spId);

                        if (subscriptionPackage == null) throw new ApiException("Subscription package not found", System.Net.HttpStatusCode.NotFound);

                        if (subscriptionPackage.IsFeatured)
                        {
                            supplier.IsFeatured = true;
                        }

                        supplier.SubscriptionPackageId = subscriptionPackage.Id;
                        supplier.SubscriptionPackage = subscriptionPackage;
                        supplier.Priority = subscriptionPackage.Priority;
                        supplier.PriorityEnd = DateTimeOffset.UtcNow.AddDays(subscriptionPackage.DurationInDays);
                        #endregion

                        #region Create invoice
                        int invoiceNumber = int.Parse(DateTimeOffset.UtcNow.ToString("ffffff"));
                        Invoice invoice = new Invoice
                        {
                            InvoiceNumber = invoiceNumber,
                            Description = order.OrderDetails?.ToList()[0].Name,
                            OrderId = order.Id,
                            Order = order,
                            UserId = account.Id,
                            User = account,
                            SupplierId = supplier.Id,
                            Supplier = supplier,
                            TotalAmount = order.TotalAmount,
                            CompanyName = Constants.InvoiceInfo.COMPANY_NAME,
                            CompanyAddress = Constants.InvoiceInfo.COMPANY_ADDRESS,
                            PhoneNumber = Constants.InvoiceInfo.PHONE_NUMBER,
                            Email = Constants.InvoiceInfo.EMAIL,
                            Website = Constants.InvoiceInfo.WEBSITE,
                        };

                        PdfDocument invoicePdf = _invoiceService.GetInvoice(invoice);

                        // Save invoice PDF to Azure Blob Storage
                        using (MemoryStream stream = new MemoryStream())
                        {
                            invoicePdf.Save(stream);
                            stream.Position = 0;

                            var formFile = new FormFile(stream, 0, stream.Length, "invoice", $"Invoice.pdf")
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = "application/pdf"
                            };

                            AzureBlobResponseDTO uploadResponse = await _storageService.UploadAsync(formFile);

                            if (uploadResponse.Error) throw new ApiException(uploadResponse?.Status!, System.Net.HttpStatusCode.InternalServerError);

                            invoice.FileUrl = uploadResponse.Blob.Uri;
                            invoice.FileName = uploadResponse.Blob.Name;
                        }

                        Invoice newInvoice = await _invoiceRepository.AddInvoiceAsync(invoice);

                        // Update invoice for order
                        order.InvoiceId = newInvoice.Id;
                        #endregion

                        #region Update database
                        await _orderRepository.UpdateOrderAsync(order);
                        await _supplierRepository.UpdateSupplierAsync(supplier);
                        #endregion

                        #region Send email to supplier
                        string emailBody = EmailTemplates.GetThankForPurchase(supplier.Name!, subscriptionPackage.Name!, Constants.InvoiceInfo.WEBSITE, Constants.Common.DOMAIN);

                        string recipientName = supplier.Name!;
                        string recipientEmail = account.Email!;
                        string subject = "Thank you for your purchase";

                        _ = Task.Run(async () => await _emailSender.SendEmailWithAttachmentAsync(recipientEmail, subject, emailBody, invoice.FileUrl!, invoice.FileName!));
                        #endregion
                    
                    return new PaymentResponse(0, "Ok", null);
                }

                return new PaymentResponse(-1, "Fail", null);

            }
            catch (ApiException ex)
            {
                return new PaymentResponse(-1, ex.Message, null);
            }
            catch (Exception ex)
            {
                return new PaymentResponse(-1, ex.Message, null);
            }


        }

        public async Task<ApplicationResponse> CreateSubscriptionPackagePaymentLink(SpPaymentRequestDTO spPaymentRequest)
        {
            try
            {
                if (!int.IsPositive(spPaymentRequest.spId)) throw new ApiException("Invalid subscription package id", System.Net.HttpStatusCode.BadRequest);

                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                Supplier? supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);

                if (supplier == null) throw new ApiException("You are not a supplier", System.Net.HttpStatusCode.NotFound);

                var subscriptionPackage = await _subscriptionPackageRepository.GetSubscriptionPackageByIdAsync(spPaymentRequest.spId);

                if (subscriptionPackage == null) throw new ApiException("Subscription package not found", System.Net.HttpStatusCode.NotFound);

                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                ItemData item = new ItemData(subscriptionPackage.Name!, 1, (int)subscriptionPackage.Price);
                List<ItemData> items = new List<ItemData> { item };

                Order order = new Order
                {
                    OrderCode = orderCode,
                    TotalAmount = subscriptionPackage.Price,
                    Status = OrderStatus.PendingPayment,
                    PaymentStatus = PaymentStatus.Pending,
                    PaymentMethod = PaymentMethod.PayOS,
                    SupplierId = supplier.Id,
                    Supplier = supplier,
                    UserId = currentUserId,
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail
                        {
                            SubscriptionPackageId = spPaymentRequest.spId,
                            Name = subscriptionPackage.Name,
                            Quantity = 1,
                            Price = subscriptionPackage.Price,
                            TotalAmount = subscriptionPackage.Price
                        }
                    }
                };

                Order newOrder = await _orderRepository.AddOrderAsync(order);

                if (newOrder == null) throw new ApiException("Failed to create order", System.Net.HttpStatusCode.InternalServerError);

                PaymentData paymentData = new PaymentData(
                    orderCode,
                    (int)subscriptionPackage.Price,
                    subscriptionPackage.Name!,
                    items,
                    this._payOsPaymentSuccessUrl,
                    this._payOsPaymentCancelUrl            
                );

                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

                return new ApplicationResponse
                {
                    Data = createPayment,
                    Message = "Payment link created successfully",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.Created
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }


        public async Task<ApplicationResponse> GetPaymentRequestInfo(int orderCode)
        {
            try
            {
                PaymentLinkInformation paymentLinkInformation = await _payOS.getPaymentLinkInformation(orderCode);

                if (paymentLinkInformation == null) throw new ApiException("Payment information not found", System.Net.HttpStatusCode.NotFound);

                return new ApplicationResponse
                {
                    Data = paymentLinkInformation,
                    Message = "Payment information retrieved successfully",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }


    }
}
