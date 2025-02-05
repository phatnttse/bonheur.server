using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;


namespace Bonheur.DAOs
{
    public class InvoiceDAO
    {
        private readonly ApplicationDbContext _context;
        public InvoiceDAO(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Invoice> AddInvoiceAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByUserIdAsync(string userId)
        {
            return await _context.Invoices.Where(i => i.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesBySupplierIdAsync(int supplierId)
        {
            return await _context.Invoices.Include(i => i.Order).ThenInclude(o => o!.OrderDetails).Include(i => i.Supplier).Include(i => i.User).Where(i => i.SupplierId == supplierId).ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(iv => iv.User)
                .Include(iv => iv.Supplier)
                .Include(iv => iv.Order)
                    .ThenInclude(o => o!.OrderDetails)
                        .ThenInclude(od => od.SubscriptionPackage)
                .Include(iv => iv.Order)           
                    .ThenInclude(o => o!.OrderDetails)
                        .ThenInclude(od => od.AdPackage)
                .FirstOrDefaultAsync(iv => iv.Id == id);
        }                  

        public async Task<Invoice> UpdateInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task DeleteInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
        }
    }
}
