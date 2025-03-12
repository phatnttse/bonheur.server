using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Bonheur.DAOs
{
    public class FavoriteSupplierDAO
    {
        private readonly ApplicationDbContext _context;

        public FavoriteSupplierDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FavoriteSupplier> AddFavoriteSupplier(FavoriteSupplier favoriteSupplier)
        {
            await _context.FavoriteSuppliers.AddAsync(favoriteSupplier);
            await _context.SaveChangesAsync();
            return await Task.FromResult(favoriteSupplier);
        }

        public async Task<IPagedList<FavoriteSupplier>> GetAllFavoriteSuppliers(string userId, int pageNumber, int pageSize)
        {
            IQueryable<FavoriteSupplier> favoriteSuppliers = _context.FavoriteSuppliers.Include(fs => fs.Supplier).ThenInclude(s => s.Category).Include(s => s.Supplier.Images).Where(fs => fs.UserId == userId);
            var orderedFavoriteSuppliers = favoriteSuppliers.OrderByDescending(a => a.CreatedAt);
            var result = orderedFavoriteSuppliers.ToPagedList(pageNumber, pageSize);
            return await Task.FromResult(result);
        }


        public async Task<IPagedList<FavoriteSupplier>> GetFavoriteSuppliersByCategoryId(string userId, int categoryId, int pageNumber, int pageSize)
        {
            IQueryable<FavoriteSupplier> favoriteSuppliers = _context.FavoriteSuppliers.Include(fs => fs.Supplier).ThenInclude(s => s.Category).Include(s => s.Supplier.Images).Where(fs => (fs.UserId == userId) && (fs.Supplier.CategoryId == categoryId));
            var orderedFavoriteSuppliers = favoriteSuppliers.OrderByDescending(a => a.CreatedAt);
            var result = orderedFavoriteSuppliers.ToPagedList(pageNumber, pageSize);
            return await Task.FromResult(result);
        }

        public async Task<FavoriteSupplier?> GetFavoriteSupplierAsync(int id) => await _context.FavoriteSuppliers.Include(fs => fs.Supplier).FirstOrDefaultAsync(fs => fs.SupplierId == id);

        public async Task<List<object>> GetFavoriteSupplierCountByCategoryAsync()
        {
            var result = await _context.FavoriteSuppliers
                .Where(f => f.Supplier != null && f.Supplier.Category != null)
                .GroupBy(f => new { f.Supplier.Category.Id, f.Supplier.Category.Name })
                .Select(g => new
                {
                    CategoryId = g.Key.Id,
                    CategoryName = g.Key.Name,
                    FavoriteCount = g.Count()
                })
                .ToListAsync();

            return result.Cast<object>().ToList(); // Trả về dưới dạng object
        }


        public async Task<FavoriteSupplier> DeleteSupplierAsync(FavoriteSupplier favoriteSupplier)
        {
            _context.FavoriteSuppliers.Remove(favoriteSupplier);
            await _context.SaveChangesAsync();
            return favoriteSupplier;
        }

        public async Task<bool> IsFavoriteSupplierAsync(string userId, int supplierId)
        {
            var exists = await _context.FavoriteSuppliers
                .AnyAsync(fs => fs.UserId == userId && fs.SupplierId == supplierId);
            return exists;
        }
    }
}
