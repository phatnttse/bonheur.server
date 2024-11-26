using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.DAOs
{
    public class SupplierCategoriesDAO
    {
        private readonly ApplicationDbContext _context;

        public SupplierCategoriesDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SupplierCategory>> GetSupplierCategoriesAsync()
        {
            var supplierCategories = await _context.SupplierCategories.ToListAsync();
            return supplierCategories;
        }

        public async Task<SupplierCategory> GetSupplierCategoryById(int supplierCategoryId)
        {
            var supplierCategory = await _context.SupplierCategories.FirstOrDefaultAsync(x => x.Id == supplierCategoryId);
            return supplierCategory;
        }

        public async Task<bool> CreateNewSupplierCategory(SupplierCategory supplierCategory)
        {
            var result = await _context.SupplierCategories.AddAsync(supplierCategory);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult>0;
        }

        public async Task<bool> UpdateSupplierCategory(SupplierCategory supplierCategory)
        {


            var existingSupplierCategory = await _context.SupplierCategories.FirstOrDefaultAsync(x => x.Id == supplierCategory.Id);

            existingSupplierCategory.Name = supplierCategory.Name;
            existingSupplierCategory.Description = supplierCategory.Description;

            _context.SupplierCategories.Update(existingSupplierCategory);
            var saveResult = await _context.SaveChangesAsync();

            return saveResult>0;

        }

        public async Task<bool> DeleteSupplierCategory(int id)
        {
                var existingCategory = await _context.SupplierCategories.FindAsync(id);
                _context.SupplierCategories.Remove(existingCategory);
                var saveResult = await _context.SaveChangesAsync();
                return saveResult>0;
        }
    }
}
