using Bonheur.BusinessObjects.Entities;
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

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers
                .Include(s => s.User)
                .Include(s => s.SupplierCategory)
                .Include(s => s.SupplierImages)
                .Include(s => s.SubscriptionPackage)
                .Include(s => s.Advertisements)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Supplier?> GetSupplierByUserIdAsync(string userId)
        {
            return await _context.Suppliers
                .Include(s => s.User)
                .Include(s => s.SupplierCategory)
                .Include(s => s.SupplierImages)
                .Include(s => s.SubscriptionPackage)
                .Include(s => s.Advertisements)
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
                .Include(s => s.User)
                .Include(s => s.SupplierCategory)
                .Include(s => s.SupplierImages)
                .Include(s => s.SubscriptionPackage);

            // Lọc theo tên nhà cung cấp
            if (!string.IsNullOrEmpty(supplierName))
            {
                query = query.Where(s => s.SupplierName!.ToLower().Trim().Contains(supplierName.ToLower().Trim()));
            }

            // Lọc theo danh mục
            if (supplierCategoryId.HasValue)
            {
                query = query.Where(s => s.SupplierCategoryId == supplierCategoryId);
            }

            // Lọc theo tỉnh thành
            if (!string.IsNullOrEmpty(province))
            {
                query = query.Where(s => s.Province!.ToLower().Trim().Contains(province.ToLower().Trim()));
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
                .OrderByDescending(s => s.BoostUntil.HasValue && s.BoostUntil > DateTime.Now) // Nhà cung cấp có gói còn hiệu lực trước
                .ThenByDescending(s => s.SubscriptionPackage != null ? s.SubscriptionPackage.PriorityLevel : 0); // Sắp xếp theo PriorityLevel của gói

            if (sortAsc.HasValue && sortAsc.Value)
            {
                orderedQuery = orderedQuery.ThenBy(s => s.SupplierName);
            }
            else if (sortAsc.HasValue && !sortAsc.Value)
            {
                orderedQuery = orderedQuery.ThenByDescending(s => s.SupplierName);
            }

            var suppliers = orderedQuery.ToPagedList(pageNumber, pageSize);

            return Task.FromResult(suppliers);
        }



        public async Task<Supplier?> CreateSupplierAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
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


    }
}







