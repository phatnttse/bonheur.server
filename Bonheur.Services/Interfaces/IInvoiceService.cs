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
        PdfDocument GetInvoice(Invoice invoice);
        Task<ApplicationResponse> GetInvoicesBySupplierAsync();
        Task<ApplicationResponse> GetInvoiceByIdAsync(int id);
    }
}
