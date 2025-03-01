using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<PdfDocument> GetInvoice(Invoice invoice);
        Task<ApplicationResponse> GetInvoicesBySupplierAsync(bool? sortAsc,
                string? orderBy, int pageNumber = 1,
                int pageSize = 10);
        Task<ApplicationResponse> GetInvoiceByIdAsync(int id);

        Task<ApplicationResponse> GetAllInvoicesAsync();
    }
}
