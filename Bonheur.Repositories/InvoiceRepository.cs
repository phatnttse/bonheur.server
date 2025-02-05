using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<Invoice>> GetInvoicesBySupplierIdAsync(int supplierId) => await _invoiceDAO.GetInvoicesBySupplierIdAsync(supplierId);
        public async Task UpdateInvoiceAsync(Invoice invoice) => await _invoiceDAO.UpdateInvoiceAsync(invoice);
    }
}
