using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using X.PagedList;
using X.PagedList.Extensions;
using Bonheur.Utils;

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
                .Include(s => s.Images)
                .Include(s => s.SubscriptionPackage)
                .SingleOrDefaultAsync(s => s.UserId == userId);

            if (supplier != null && supplier.Images != null)
            {
                supplier.Images = supplier.Images.OrderByDescending(image => image.IsPrimary).ToList();
            }

            return supplier;
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
            var suppliers = _context.Suppliers
                .Include(s => s.Category)
                .Include(s => s.Images)
                .Where(s => s.Status == SupplierStatus.APPROVED)
                .Where(s => string.IsNullOrEmpty(supplierName) || s.Name!.ToLower().Contains(supplierName.ToLower()))
                .Where(s => !supplierCategoryId.HasValue || s.CategoryId == supplierCategoryId)
                .Where(s => string.IsNullOrEmpty(province) || s.Province!.ToLower().Contains(province.ToLower()))
                .Where(s => !isFeatured.HasValue || s.IsFeatured == isFeatured)
                .Where(s => !averageRating.HasValue || s.AverageRating >= averageRating)
                .Where(s => !minPrice.HasValue || s.Price >= minPrice)
                .Where(s => !maxPrice.HasValue || s.Price <= maxPrice)
                .OrderByDescending(s => s.ProrityEnd.HasValue && s.ProrityEnd > DateTimeOffset.UtcNow)
                .ThenByDescending(s => s.ProrityEnd.HasValue && s.ProrityEnd > DateTimeOffset.UtcNow ? s.Priority : 0)
                .ThenByDescending(s => s.Priority)
                .ThenBy(s => sortAsc.HasValue && sortAsc.Value ? s.Name : null)
                .ThenByDescending(s => sortAsc.HasValue && !sortAsc.Value ? s.Name : null)
                .AsEnumerable()
                .Select(s =>
                {
                    s.Images = s.Images?.OrderByDescending(image => image.IsPrimary).ToList() ?? new List<SupplierImage>();
                    return s;
                })
                .ToPagedList(pageNumber, pageSize);

            return Task.FromResult(suppliers);
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
                int pageNumber = 1,
                int pageSize = 10
        )
        {
            var suppliers = _context.Suppliers
                .Include(s => s.Category)
                .Include(s => s.Images)
                .Where(s => !status.HasValue || s.Status == status)
                .Where(s => string.IsNullOrEmpty(supplierName) || s.Name!.ToLower().Contains(supplierName.ToLower()))
                .Where(s => !supplierCategoryId.HasValue || s.CategoryId == supplierCategoryId)
                .Where(s => string.IsNullOrEmpty(province) || s.Province!.ToLower().Contains(province.ToLower()))
                .Where(s => !isFeatured.HasValue || s.IsFeatured == isFeatured)
                .Where(s => !averageRating.HasValue || s.AverageRating >= averageRating)
                .Where(s => !minPrice.HasValue || s.Price >= minPrice)
                .Where(s => !maxPrice.HasValue || s.Price <= maxPrice)
                .OrderByDescending(s => s.ProrityEnd.HasValue && s.ProrityEnd > DateTimeOffset.UtcNow)
                .ThenByDescending(s => s.ProrityEnd.HasValue && s.ProrityEnd > DateTimeOffset.UtcNow ? s.Priority : 0)
                .ThenByDescending(s => s.Priority)
                .ThenBy(s => sortAsc.HasValue && sortAsc.Value ? s.Name : null)
                .ThenByDescending(s => sortAsc.HasValue && !sortAsc.Value ? s.Name : null)
                .AsEnumerable()
                .Select(s =>
                {
                    s.Images = s.Images?.OrderByDescending(image => image.IsPrimary).ToList() ?? new List<SupplierImage>();
                    return s;
                })
                .ToPagedList(pageNumber, pageSize);

            return Task.FromResult(suppliers);
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
                .Include(s => s.Images)
                .SingleOrDefaultAsync(s => s.Slug == slug && s.Status == SupplierStatus.APPROVED);
        }

    }
}







