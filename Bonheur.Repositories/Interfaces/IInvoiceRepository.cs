using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> AddInvoiceAsync(Invoice invoice);
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetInvoicesByUserIdAsync(string userId);
        Task<IPagedList<Invoice>> GetInvoicesBySupplierIdAsync(int supplierId, bool? sortAsc,
                string? orderBy, int pageNumber = 1,
                int pageSize = 10);
        Task UpdateInvoiceAsync(Invoice invoice);
        Task DeleteInvoiceAsync(Invoice invoice);
        Task<List<Invoice>> GetAllInvoicesAsync();
    }
}
