using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Invoice;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using X.PagedList;

namespace Bonheur.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public InvoiceService(IInvoiceRepository invoiceRepository, ISupplierRepository supplierRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public PdfDocument GetInvoice(Invoice invoice)
        {
            var document = new Document();

            BuildDocument(document, invoice);

            var renderer = new PdfDocumentRenderer();

            renderer.Document = document;

            renderer.RenderDocument();

            return renderer.PdfDocument;
        }

        private void BuildDocument(Document document, Invoice invoice)
        {
            Section section = document.AddSection();

            section.PageSetup.TopMargin = Unit.FromCentimeter(2); // 2 cm trên
            section.PageSetup.BottomMargin = Unit.FromCentimeter(2); // 2 cm dưới
            section.PageSetup.LeftMargin = Unit.FromCentimeter(2); // 2 cm trái
            section.PageSetup.RightMargin = Unit.FromCentimeter(3); // 2 cm phải

            // Header Table for company name and logo
            var headerTable = section.AddTable();
            headerTable.Borders.Visible = false;

            // Columns for company details and logo
            headerTable.AddColumn("12cm"); // Left column for company details
            headerTable.AddColumn("8cm");  // Right column for logo

            Row headerRow = headerTable.AddRow();

            // Left cell for company details
            var leftCell = headerRow.Cells[0];
            var paragraph = leftCell.AddParagraph();
            var title = paragraph.AddFormattedText(invoice.CompanyName!, TextFormat.Bold);
            title.Font.Size = 18;
            title.Font.Color = Colors.LightPink;
            paragraph.Format.SpaceAfter = 10;
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.Format.Font.Size = 10;
            paragraph.AddText(invoice.CompanyAddress!);
            paragraph.AddLineBreak();
            paragraph.AddText($"Website: {invoice.Website}");
            paragraph.AddLineBreak();
            paragraph.AddText($"Contact: {invoice.PhoneNumber}");
            paragraph.AddLineBreak();
            paragraph.AddText($"Email: {invoice.Email}");

            // Right cell for logo
            var rightCell = headerRow.Cells[1];
            //var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logo.png");
            var logo = rightCell.AddImage(Constants.InvoiceInfo.LOGO_URL);

            logo.Width = "4cm";
            logo.Height = "4cm";
            rightCell.Format.Alignment = ParagraphAlignment.Left;

            // Billed To and Invoice Section
            var billInvoiceTable = section.AddTable();
            billInvoiceTable.Borders.Visible = false;

            // Columns for Billed To and Invoice Info
            billInvoiceTable.AddColumn("10cm");
            billInvoiceTable.AddColumn("10cm");

            var billInvoiceRow = billInvoiceTable.AddRow();

            // Billed To Cell
            var billedToCell = billInvoiceRow.Cells[0];
            var billedParagraph = billedToCell.AddParagraph();
            var billedTo = billedParagraph.AddFormattedText("BILLED TO", TextFormat.Bold);
            billedTo.Font.Size = 14;
            billedParagraph.AddLineBreak();
            billedParagraph.AddLineBreak();
            billedParagraph.Format.Font.Size = 10;
            billedParagraph.AddText(invoice.Supplier!.Name!);
            billedParagraph.AddLineBreak();
            billedParagraph.AddText($"{invoice.Supplier.Address}");
            billedParagraph.AddLineBreak();
            billedParagraph.AddText($"Phone: {invoice.Supplier.PhoneNumber}");
            billedParagraph.AddLineBreak();
            billedParagraph.AddText($"Email: {invoice.User!.Email}");

            // Invoice Info Cell
            var invoiceCell = billInvoiceRow.Cells[1];
            var invoiceParagraph = invoiceCell.AddParagraph();
            invoiceParagraph.AddText($"Invoice no: {invoice.InvoiceNumber}");
            invoiceParagraph.AddLineBreak();
            invoiceParagraph.Add(new DateField { Format = "yyyy/MM/dd HH:mm:ss"});
            invoiceParagraph.Format.SpaceBefore = 3;
            invoiceParagraph.Format.Alignment = ParagraphAlignment.Center;

            section.AddParagraph().Format.SpaceAfter = 15;

            // Products Table
            var table = document.LastSection.AddTable();
            table.Borders.Width = 0.5;
            table.Borders.Color = Colors.LightPink;
            table.Rows.LeftIndent = 0;

            // Add Columns for Table
            table.AddColumn("2.5cm"); // No
            table.AddColumn("8cm");  // Description
            table.AddColumn("2.5cm"); // Qty
            table.AddColumn("3cm");  // Price
            table.AddColumn("3cm");  // Amount
            table.Rows.Height = "0.8cm";


            // Header Row
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightPink;
            row.Cells[0].AddParagraph("Order no");
            row.Cells[1].AddParagraph("Product name");
            row.Cells[2].AddParagraph("Quantity");
            row.Cells[3].AddParagraph("Price");
            row.Cells[4].AddParagraph("Amount");
            row.Format.Alignment = ParagraphAlignment.Center; // Căn giữa header
            row.VerticalAlignment = VerticalAlignment.Center; // Căn giữa theo chiều dọc

            foreach (var orderDetail in invoice.Order!.OrderDetails!)
            {
                row = table.AddRow();
                row.Height = "1cm"; // Increase row height
                row.Format.Alignment = ParagraphAlignment.Center; // Căn giữa header
                row.VerticalAlignment = VerticalAlignment.Center; // Căn giữa theo chiều dọc
                row.Cells[0].AddParagraph(orderDetail.Order!.OrderCode!.ToString());
                row.Cells[1].AddParagraph(orderDetail.Name!.ToString() ?? "N/A");
                row.Cells[2].AddParagraph(orderDetail.Quantity.ToString());
                row.Cells[3].AddParagraph(Utilities.FormatCurrency(orderDetail.Price));
                row.Cells[4].AddParagraph(Utilities.FormatCurrency(orderDetail.TotalAmount));
            }
          
            // Subtotal, Tax, and Total in table
            row = table.AddRow();
            row.Height = "1cm";          
            row.Cells[0].MergeRight = 2; // Merge first three columns
            row.Cells[0].AddParagraph(""); // Empty space
            row.Cells[3].AddParagraph("Subtotal");
            row.Cells[4].AddParagraph(Utilities.FormatCurrency(invoice.Order.TotalAmount - invoice.TaxAmount));
            row.Format.Alignment = ParagraphAlignment.Center; // Căn giữa header
            row.VerticalAlignment = VerticalAlignment.Center; // Căn giữa theo chiều dọc

            row = table.AddRow();
            row.Height = "1cm";
            row.Cells[0].MergeRight = 2;
            row.Cells[0].AddParagraph("");
            row.Cells[3].AddParagraph("Tax");
            row.Cells[4].AddParagraph(Utilities.FormatCurrency(invoice.TaxAmount));
            row.Format.Alignment = ParagraphAlignment.Center; // Căn giữa header
            row.VerticalAlignment = VerticalAlignment.Center; // Căn giữa theo chiều dọc

            row = table.AddRow();
            row.Height = "1cm";
            row.Cells[0].MergeRight = 2;
            row.Cells[0].AddParagraph("");
            row.Cells[3].AddParagraph("Total");
            row.Cells[4].AddParagraph(Utilities.FormatCurrency(invoice.Order.TotalAmount));
            row.Cells[4].Format.Font.Bold = true;
            row.Format.Alignment = ParagraphAlignment.Center; // Căn giữa header
            row.VerticalAlignment = VerticalAlignment.Center; // Căn giữa theo chiều dọc

            // Footer
            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText($"{invoice.CompanyName} . {invoice.CompanyAddress}");
            paragraph.Format.Alignment = ParagraphAlignment.Center;

        }

        public async Task<ApplicationResponse> GetInvoicesBySupplierAsync(bool? sortAsc,
                string? orderBy, int pageNumber = 1,
                int pageSize = 10)
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                Supplier? supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);

                if (supplier == null) throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                IPagedList<Invoice> invoices = await _invoiceRepository.GetInvoicesBySupplierIdAsync(supplier!.Id, sortAsc, orderBy, pageNumber, pageSize);

                var invoicesDTO = _mapper.Map<List<InvoiceDTO>>(invoices);

                var responseData = new PagedData<InvoiceDTO>
                {
                    Items = invoicesDTO,
                    PageNumber = invoices.PageNumber,
                    PageSize = invoices.PageSize,
                    TotalItemCount = invoices.TotalItemCount,
                    PageCount = invoices.PageCount,
                    IsFirstPage = invoices.IsFirstPage,
                    IsLastPage = invoices.IsLastPage,
                    HasNextPage = invoices.HasNextPage,
                    HasPreviousPage = invoices.HasPreviousPage
                };

                return new ApplicationResponse
                {
                    Message = $"Invoices for supplier {supplier!.Name} found",
                    Data = responseData,
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

        public Task<ApplicationResponse> GetInvoiceByIdAsync(int id)
        {
            try
            {
                Invoice? invoice = _invoiceRepository.GetInvoiceByIdAsync(id).Result;

                if (invoice == null) throw new ApiException("Invoice not found", System.Net.HttpStatusCode.NotFound);

                return Task.FromResult(new ApplicationResponse
                {
                    Message = $"Invoice {invoice!.InvoiceNumber} found",
                    Data = _mapper.Map<InvoiceDetailDTO>(invoice),
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                });

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

        public async Task<ApplicationResponse> GetAllInvoicesAsync()
        {
            try
            {
                List<Invoice> invoices = await _invoiceRepository.GetAllInvoicesAsync();

                return new ApplicationResponse
                {
                    Message = "Invoices retrieved successfully",
                    Data = _mapper.Map<List<InvoiceDTO>>(invoices),
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
