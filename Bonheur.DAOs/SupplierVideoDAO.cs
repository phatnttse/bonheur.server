using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.DAOs
{
    public class SupplierVideoDAO
    {
        private readonly ApplicationDbContext _context;

        public SupplierVideoDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SupplierVideo>> GetSupplierVideosBySupplierIdAsync(int supplierId)
        {
            return await _context.SupplierVideos.Where(sv => sv.SupplierId == supplierId).ToListAsync();
        }

        public async Task<SupplierVideo?> GetSupplierVideoById(int id)
        {
            return await _context.SupplierVideos.FindAsync(id);
        }

        public async Task AddSupplierVideoAsync(SupplierVideo supplierVideo)
        {
            await _context.SupplierVideos.AddAsync(supplierVideo);
            await _context.SaveChangesAsync();
        }

        public async Task AddSupplierVideoRangeAsync(IEnumerable<SupplierVideo> supplierVideos)
        {
            await _context.SupplierVideos.AddRangeAsync(supplierVideos);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSupplierVideoAsync(SupplierVideo supplierVideo)
        {

            _context.SupplierVideos.Remove(supplierVideo);
            await _context.SaveChangesAsync();
            
        }

        public async Task DeleteSupplierVideosBySupplierIdAsync(int supplierId)
        {
            var supplierVideos = await _context.SupplierVideos.Where(sv => sv.SupplierId == supplierId).ToListAsync();
            if (supplierVideos.Count > 0)
            {
                _context.SupplierVideos.RemoveRange(supplierVideos);
                await _context.SaveChangesAsync();
            }
        }
    }
}
