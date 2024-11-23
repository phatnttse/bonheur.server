using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.DAOs
{
    public class SupplierImageDAO
    {
        private readonly ApplicationDbContext _context;

        public SupplierImageDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddSupplierImageAsync(SupplierImage supplierImage)
        {
            await _context.SupplierImages.AddAsync(supplierImage);
            await _context.SaveChangesAsync();
        }

        public async Task AddSupplierImagesAsync(IEnumerable<SupplierImage> supplierImages)
        {
            await _context.SupplierImages.AddRangeAsync(supplierImages);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSupplierImageAsync(int id)
        {
            var supplierImage = await _context.SupplierImages.FindAsync(id);
            if (supplierImage != null)
            {
                _context.SupplierImages.Remove(supplierImage);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteSupplierImagesBySupplierIdAsync(int supplierId)
        {
            var supplierImages = await _context.SupplierImages.Where(si => si.SupplierId == supplierId).ToListAsync();
            if (supplierImages.Count > 0)
            {
                _context.SupplierImages.RemoveRange(supplierImages);
                await _context.SaveChangesAsync();
            }
        }
    }
}
