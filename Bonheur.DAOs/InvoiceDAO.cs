using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using X.PagedList;
using X.PagedList.Extensions;


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

        public Task<IPagedList<Invoice>> GetInvoicesBySupplierIdAsync(int supplierId, bool? sortAsc,
                string? orderBy, int pageNumber = 1,
                int pageSize = 10)
        {
            var invoices = _context.Invoices
                .Include(i => i.Order)
                .ThenInclude(o => o!.OrderDetails)
                .Include(i => i.Supplier)
                .Include(i => i.User)
                .Where(i => i.SupplierId == supplierId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(o => o.CreatedAt);

            if (!string.IsNullOrEmpty(orderBy))
            {
                // Tạo biểu thức cho orderBy
                var parameter = Expression.Parameter(typeof(Invoice), "s");
                var property = Expression.Property(parameter, orderBy);
                var lambda = Expression.Lambda<Func<Invoice, object>>(Expression.Convert(property, typeof(object)), parameter);

                // Áp dụng sắp xếp
                invoices = sortAsc == true
                    ? invoices.OrderBy(lambda)
                    : invoices.OrderByDescending(lambda);
            }

            return Task.FromResult(invoices.ToPagedList(pageNumber, pageSize));
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

        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoices.ToListAsync();
        }
    }
}
