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
    public class AdvertisementDAO
    {
        private readonly ApplicationDbContext _context;
        public AdvertisementDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAdvertisement(Advertisement advertisement)
        {
            _context.Advertisements.Add(advertisement);
            await _context.SaveChangesAsync();
        }

        public async Task<IPagedList<Advertisement>> GetAdvertisements(string? searchTitle, string? searchContent, int pageNumber = 1, int pageSize = 10)
        {
            IQueryable<Advertisement> query = _context.Advertisements;
            if (!string.IsNullOrEmpty(searchTitle)) {
                query = query.Where(a => EF.Functions.Like(a.Title, $"%{a.Title}%"));
            }
            if (!string.IsNullOrEmpty(searchContent)) {
                query = query.Where(a => EF.Functions.Like(a.Content, $"%{a.Content}%" )); 
            }
            var orderedQuery = query.OrderByDescending(a => a.CreatedAt);
            var advertisementPaginated = orderedQuery.ToPagedList(pageNumber, pageSize);
            return await Task.FromResult(advertisementPaginated);
        }

        public async Task<Advertisement?> GetAdvertisementById(int id)
        {
            return await _context.Advertisements
                .Include(a => a.Supplier)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAdvertisement(Advertisement advertisement)
        {
            _context.Advertisements.Update(advertisement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAdvertisement(Advertisement advertisement)
        {
            _context.Advertisements.Remove(advertisement);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalAdvertisementsCount()
        {
            return await _context.Advertisements.CountAsync();
        }

        public async Task<IPagedList<Advertisement>> GetAdvertisementsBySupplier(int supplierId, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _context.Advertisements
                .Where(a => a.SupplierId == supplierId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return result.ToPagedList(pageNumber, pageSize);
        }
    }
}
