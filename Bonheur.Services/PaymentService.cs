using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Http;
using Net.payOS;
using Net.payOS.Types;


namespace Bonheur.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOS _payOS;
        private readonly ISubscriptionPackageRepository _subscriptionPackageRepository;
        private readonly string _payOsPaymentSuccessUrl;
        private readonly string _payOsPaymentCancelUrl;


        public PaymentService(PayOS payOS, ISubscriptionPackageRepository subscriptionPackageRepository)
        {
            _payOS = payOS;
            _subscriptionPackageRepository = subscriptionPackageRepository;
            _payOsPaymentSuccessUrl = Environment.GetEnvironmentVariable("PAYOS_PAYMENT_SUCCESS_URL") ?? throw new Exception("Environtment string 'PAYOS_PAYMENT_SUCCESS_URL' not found");
            _payOsPaymentCancelUrl = Environment.GetEnvironmentVariable("PAYOS_PAYMENT_CANCEL_URL") ?? throw new Exception("Environtment string 'PAYOS_PAYMENT_CANCEL_URL' not found");
        }

        public async Task<ApplicationResponse> subscriptionPackagePayment(int subscriptionPackageId)
        {
            try
            {
                if (subscriptionPackageId <= 0) throw new ApiException("Invalid subscription package id", System.Net.HttpStatusCode.BadRequest);

                var subscriptionPackage = await _subscriptionPackageRepository.GetSubscriptionPackageByIdAsync(subscriptionPackageId);

                if (subscriptionPackage == null) throw new ApiException("Subscription package not found", System.Net.HttpStatusCode.NotFound);

                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                ItemData item = new ItemData(subscriptionPackage.Name!, 1, (int)subscriptionPackage.Price);
                List<ItemData> items = new List<ItemData> { item };

                PaymentData paymentData = new PaymentData(
                    orderCode,
                    (int)subscriptionPackage.Price,
                    "Pay for subscription",
                    items,
                    this._payOsPaymentCancelUrl,
                    this._payOsPaymentSuccessUrl
                );

                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

                return new ApplicationResponse
                {
                    Data = createPayment,
                    Message = "Payment link created successfully",
                    Success = true
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
