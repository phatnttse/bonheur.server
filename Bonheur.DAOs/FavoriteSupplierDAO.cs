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

        public async Task<FavoriteSupplier?> GetFavoriteSupplierAsync(int id) => await _context.FavoriteSuppliers.Include(fs => fs.Supplier).FirstOrDefaultAsync(fs => fs.Id == id);

        public async Task<FavoriteSupplier> UpdateFavoriteSupplierAsync(FavoriteSupplier favoriteSupplier)
        {
            _context.FavoriteSuppliers.Update(favoriteSupplier);
            await _context.SaveChangesAsync();
            return favoriteSupplier;
        }

        public async Task<FavoriteSupplier> DeleteSupplierAsync(FavoriteSupplier favoriteSupplier)
        {
            _context.FavoriteSuppliers.Remove(favoriteSupplier);
            await _context.SaveChangesAsync();
            return favoriteSupplier;
        }
    }
}
