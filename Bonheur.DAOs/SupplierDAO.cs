using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace Bonheur.DAOs
{
    public class SupplierDAO
    {
        private readonly ApplicationDbContext _context;

        public SupplierDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id, bool isIncludeUser)
        {
            IQueryable<Supplier> query = _context.Suppliers
                .Include(s => s.Category)
                .Include(s => s.Images);

            if (isIncludeUser)
            {
                query = query.Include(s => s.User);
            }

            return await query.SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Supplier?> GetSupplierByUserIdAsync(string userId)
        {
            return await _context.Suppliers
                .Include(s => s.User)
                .Include(s => s.Category)
                .Include(s => s.Images)
                .Include(s => s.SubscriptionPackage)
                .SingleOrDefaultAsync(s => s.UserId == userId);
        }

        public Task<IPagedList<Supplier>> GetSuppliersAsync(
                string? supplierName,
                int? supplierCategoryId,
                string? province,
                bool? isFeatured,
                decimal? averageRating,
                decimal? minPrice,
                decimal? maxPrice,
                bool? sortAsc,
                int pageNumber = 1,
                int pageSize = 10
        )
        {
            IQueryable<Supplier> query = _context.Suppliers
                .Include(s => s.Category)
                .Include(s => s.Images)
                .Where(s => s.Status == SupplierStatus.APPROVED);

            // Lọc theo tên nhà cung cấp
            if (!string.IsNullOrEmpty(supplierName))
            {
                query = query.Where(s => EF.Functions.Like(s.Name, $"%{supplierName}%"));
            }

            // Lọc theo danh mục
            if (supplierCategoryId.HasValue)
            {
                query = query.Where(s => s.CategoryId == supplierCategoryId);
            }

            // Lọc theo tỉnh thành
            if (!string.IsNullOrEmpty(province))
            {
                query = query.Where(s => EF.Functions.Like(s.Province, $"%{province}%"));
            }

            // Lọc theo trạng thái nổi bật
            if (isFeatured.HasValue)
            {
                query = query.Where(s => s.IsFeatured == isFeatured);
            }

            // Lọc theo đánh giá
            if (averageRating.HasValue)
            {
                query = query.Where(s => s.AverageRating >= averageRating);
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(s => s.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(s => s.Price <= maxPrice);
            }

            IOrderedQueryable<Supplier> orderedQuery = query
            .OrderByDescending(s => s.ProrityEnd.HasValue && s.ProrityEnd > DateTimeOffset.UtcNow)
            .ThenByDescending(s => s.ProrityEnd.HasValue && s.ProrityEnd > DateTimeOffset.UtcNow ? s.Priority : 0)
            .ThenByDescending(s => s.Priority);

            if (sortAsc.HasValue && sortAsc.Value)
            {
                orderedQuery = orderedQuery.ThenBy(s => s.Name);
            }
            else if (sortAsc.HasValue && !sortAsc.Value)
            {
                orderedQuery = orderedQuery.ThenByDescending(s => s.Name);
            }

            var suppliers = orderedQuery.ToPagedList(pageNumber, pageSize);

            return Task.FromResult(suppliers);
        }



        public async Task<Supplier?> CreateSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier?> UpdateSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
                return false;

            _context.Suppliers.Remove(supplier);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> IsSupplierAsync(string userId)
        {
            return await _context.Suppliers.AnyAsync(s => s.UserId == userId);
        }

    }
}







