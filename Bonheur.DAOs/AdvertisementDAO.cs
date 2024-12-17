using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<Advertisement>> GetAdvertisements()
        {
            return await _context.Advertisements
                .Include(a => a.Supplier)
                .ToListAsync();
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
    }
}
