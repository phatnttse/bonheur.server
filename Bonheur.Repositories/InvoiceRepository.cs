using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceDAO _invoiceDAO;
        public InvoiceRepository(InvoiceDAO invoiceDAO)
        {
            _invoiceDAO = invoiceDAO;
        }

        public async Task<Invoice> AddInvoiceAsync(Invoice invoice) => await _invoiceDAO.AddInvoiceAsync(invoice);
        public async Task DeleteInvoiceAsync(Invoice invoice) => await _invoiceDAO.DeleteInvoiceAsync(invoice);
        public async Task<Invoice?> GetInvoiceByIdAsync(int id) => await _invoiceDAO.GetInvoiceByIdAsync(id);
        public async Task<IEnumerable<Invoice>> GetInvoicesByUserIdAsync(string userId) => await _invoiceDAO.GetInvoicesByUserIdAsync(userId);
        public async Task<IPagedList<Invoice>> GetInvoicesBySupplierIdAsync(int supplierId, bool? sortAsc,
                string? orderBy, int pageNumber = 1,
                int pageSize = 10) => await _invoiceDAO.GetInvoicesBySupplierIdAsync(supplierId, sortAsc, orderBy, pageNumber, pageSize);
        public async Task UpdateInvoiceAsync(Invoice invoice) => await _invoiceDAO.UpdateInvoiceAsync(invoice);
        public async Task<List<Invoice>> GetAllInvoicesAsync() => await _invoiceDAO.GetAllInvoicesAsync();

    }
}
