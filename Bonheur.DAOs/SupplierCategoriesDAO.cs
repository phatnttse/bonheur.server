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

        public async Task<(bool Succeded, object Response)> CreateNewSupplierCategory(SupplierCategory supplierCategory)
        {
            try
            {
                var result = await _context.SupplierCategories.AddAsync(supplierCategory);
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    return (false, new{message = "Fail to add new supplier category!"});
                }
                return (true, new{category = supplierCategory });
            }
            catch (Exception ex)
            {
                return (false, new { message = $"Fail to add new supplier category! Because: {ex.Message}" });
                throw;
            }
        }

        public async Task<(bool Succeeded, object Response)> UpdateSupplierCategory(SupplierCategory supplierCategory)
        {
            try
            {
                if (supplierCategory == null) {
                    return (false, new { message = "Invalid supplier category data." });
                }

                var existingSupplierCategory = await _context.SupplierCategories.FirstOrDefaultAsync(x => x.Id == supplierCategory.Id);

                if (existingSupplierCategory == null)
                {
                    return (false, new { message = "Supplier category not found!" });

                }

                existingSupplierCategory.Name = supplierCategory.Name;
                existingSupplierCategory.Description = supplierCategory.Description;

                _context.SupplierCategories.Update(existingSupplierCategory);
                await _context.SaveChangesAsync();

                return (true, existingSupplierCategory);    
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Succeeded, object Response)> DeleteSupplierCategory(int id)
        {
            try
            {
                var existingCategory = await _context.SupplierCategories.FindAsync(id);
                if (existingCategory == null)
                {
                    return (false, new {message= $"Supplier category with id {id} does not exist!" });
                }
                _context.SupplierCategories.Remove(existingCategory);
                await _context.SaveChangesAsync();
                return (true, new {message= $"Delete category with id {id} successfully!"});
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }
    }
}
