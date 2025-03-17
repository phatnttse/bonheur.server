using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
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

        public async Task<IPagedList<Advertisement>> GetAdvertisements(string? searchTitle, string? searchContent, AdvertisementStatus? status, PaymentStatus? paymentStatus, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _context.Advertisements
                .Include(a => a.Supplier)
                .Include(a => a.AdPackage)
                .Where(s => string.IsNullOrEmpty(searchTitle) || s.Title!.ToLower().Contains(searchTitle.ToLower()))
                .Where(s => string.IsNullOrEmpty(searchContent) || s.Content!.ToLower().Contains(searchContent.ToLower()))
                .Where(s => !status.HasValue || s.Status == status.Value)
                .Where(s => !paymentStatus.HasValue || s.PaymentStatus == paymentStatus.Value)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            return result.ToPagedList(pageNumber, pageSize);
        }

        public async Task<Advertisement?> GetAdvertisementById(int id)
        {
            return await _context.Advertisements
                .Include(a => a.Supplier)
                .Include(a => a.AdPackage)
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

        public async Task<IPagedList<Advertisement>> GetActiveAdvertisements(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _context.Advertisements
                .Include(a => a.Supplier)
                .Include(a => a.AdPackage)
                .Where(a => a.Status == AdvertisementStatus.Approved && a.PaymentStatus == PaymentStatus.Paid && a.IsActive && a.AdPackage.StartDate > DateTimeOffset.UtcNow && a.AdPackage.EndDate < DateTimeOffset.UtcNow)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return result.ToPagedList(pageNumber, pageSize);
        }
    }
}
