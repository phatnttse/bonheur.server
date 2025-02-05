using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> AddInvoiceAsync(Invoice invoice);
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetInvoicesByUserIdAsync(string userId);
        Task<IEnumerable<Invoice>> GetInvoicesBySupplierIdAsync(int supplierId);
        Task UpdateInvoiceAsync(Invoice invoice);
        Task DeleteInvoiceAsync(Invoice invoice);
    }
}
