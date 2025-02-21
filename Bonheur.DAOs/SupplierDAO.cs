using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using X.PagedList;
using X.PagedList.Extensions;
using Bonheur.Utils;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Drawing.Charts;

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

            var supplier = await query.SingleOrDefaultAsync(s => s.Id == id);

            if (supplier != null && supplier.Images != null)
            {
                supplier.Images = supplier.Images.OrderByDescending(image => image.IsPrimary).ToList();
            }

            return supplier;
        }

        public async Task<Supplier?> GetSupplierByUserIdAsync(string userId)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.User)
                .Include(s => s.Category)
                .Include(s => s.Images!.OrderByDescending(img => img.IsPrimary))
                .Include(s => s.SubscriptionPackage)
                .SingleOrDefaultAsync(s => s.UserId == userId);
        
            return supplier;
        }

        public Task<IPagedList<Supplier>> GetSuppliersAsync(
                string? supplierName,
                List<int>? supplierCategoryIds,
                string? province,
                bool? isFeatured,
                decimal? averageRating,
                decimal? minPrice,
                decimal? maxPrice,
                bool? sortAsc,
                string? orderBy,
                int pageNumber = 1,
                int pageSize = 10
        )
        {
            var suppliers = _context.Suppliers
                .Include(s => s.Category)
                .Include(s => s.Images!.OrderByDescending(img => img.IsPrimary))
                .Include(s => s.SubscriptionPackage)
                .Where(s => s.Status == SupplierStatus.Approved)
                .Where(s => string.IsNullOrEmpty(supplierName) || s.Name!.ToLower().Contains(supplierName.ToLower()))
                .Where(s => supplierCategoryIds == null || !supplierCategoryIds.Any() || supplierCategoryIds.Contains(s.CategoryId)) 
                .Where(s => string.IsNullOrEmpty(province) || s.Province!.ToLower().Contains(province.ToLower()))
                .Where(s => !isFeatured.HasValue || s.IsFeatured == isFeatured)
                .Where(s => !averageRating.HasValue || s.AverageRating >= averageRating)
                .Where(s => !minPrice.HasValue || s.Price >= minPrice)
                .Where(s => !maxPrice.HasValue || s.Price <= maxPrice)
                .OrderByDescending(s => s.PriorityEnd ?? DateTimeOffset.MinValue)
                .ThenByDescending(s => s.Priority)
                .ThenByDescending(s => s.AverageRating)
                .ThenBy(s => s.Id);

            if (!string.IsNullOrEmpty(orderBy))
            {
                // Tạo biểu thức cho orderBy
                var parameter = Expression.Parameter(typeof(Supplier), "s");
                var property = Expression.Property(parameter, orderBy);
                var lambda = Expression.Lambda<Func<Supplier, object>>(Expression.Convert(property, typeof(object)), parameter);

                // Áp dụng sắp xếp
                suppliers = sortAsc == true
                    ? suppliers.OrderBy(lambda)
                    : suppliers.OrderByDescending(lambda);
            }

            var result = suppliers
                .ToPagedList(pageNumber, pageSize);

             return Task.FromResult(result);
        }

        public Task<IPagedList<Supplier>> GetSuppliersByAdminAsync(
                string? supplierName,
                int? supplierCategoryId,
                string? province,
                bool? isFeatured,
                decimal? averageRating,
                decimal? minPrice,
                decimal? maxPrice,
                SupplierStatus? status,
                bool? sortAsc,
                string? orderBy,
                int pageNumber = 1,
                int pageSize = 10
        )
        {
            var suppliers = _context.Suppliers
                .Include(s => s.Category)
                .Include(s => s.Images!.OrderByDescending(img => img.IsPrimary))
                .Where(s => !status.HasValue || s.Status == status)
                .Where(s => string.IsNullOrEmpty(supplierName) || s.Name!.ToLower().Contains(supplierName.ToLower()))
                .Where(s => !supplierCategoryId.HasValue || s.CategoryId == supplierCategoryId)
                .Where(s => string.IsNullOrEmpty(province) || s.Province!.ToLower().Contains(province.ToLower()))
                .Where(s => !isFeatured.HasValue || s.IsFeatured == isFeatured)
                .Where(s => !averageRating.HasValue || s.AverageRating >= averageRating)
                .Where(s => !minPrice.HasValue || s.Price >= minPrice)
                .Where(s => !maxPrice.HasValue || s.Price <= maxPrice)
                .OrderByDescending(s => s.PriorityEnd ?? DateTimeOffset.MinValue)
                .ThenByDescending(s => s.Priority)
                .ThenByDescending(s => s.AverageRating)
                .ThenBy(s => s.Id);

            if (!string.IsNullOrEmpty(orderBy))
            {
                // Tạo biểu thức cho orderBy
                var parameter = Expression.Parameter(typeof(Supplier), "s");
                var property = Expression.Property(parameter, orderBy);
                var lambda = Expression.Lambda<Func<Supplier, object>>(Expression.Convert(property, typeof(object)), parameter);

                // Áp dụng sắp xếp
                suppliers = sortAsc == true
                    ? suppliers.OrderBy(lambda)
                    : suppliers.OrderByDescending(lambda);
            }

            var result = suppliers
                .ToPagedList(pageNumber, pageSize);

            return Task.FromResult(result);
        }

        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers
                .Include(s => s.Category)
                .ToListAsync();
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

        public async Task<Supplier?> GetSupplierBySlugAsync(string slug)
        {
            return await _context.Suppliers
                .Include(s => s.Category)
                .Include(s => s.Images!.OrderByDescending(img => img.IsPrimary))
                .Include(s => s.SubscriptionPackage)            
                .Include(s => s.Faqs)
                .Include(s => s.Reviews)
                .ThenInclude(rv => rv.User)
                .Include(s => s.SocialNetworks)
                .ThenInclude(ssn => ssn.SocialNetwork)
                .SingleOrDefaultAsync(s => s.Slug == slug && s.Status == SupplierStatus.Approved);
        }

    }
}







