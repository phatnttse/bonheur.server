using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bonheur.DAOs
{
    public class SupplierFAQDAO
    {
        private readonly ApplicationDbContext _dbcontext;

        public SupplierFAQDAO(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<IEnumerable<SupplierFAQ>> GetAllAsync()
        {
            return await _dbcontext.SupplierFAQs.ToListAsync();
        }

        public async Task<SupplierFAQ?> GetByIdAsync(int id)
        {
            return await _dbcontext.SupplierFAQs.FindAsync(id);
        }

        public async Task<IEnumerable<SupplierFAQ>> GetSupplierFAQsBySupplierIdAsync(int supplierId)
        {
            return await _dbcontext.SupplierFAQs.Where(s => s.SupplierId == supplierId).ToListAsync();
        }

        public async Task<List<SupplierFAQ>> GetByIdsAsync(List<int> ids)
        {
            return await _dbcontext.SupplierFAQs.Where(s => ids.Contains(s.Id)).ToListAsync();
        }

        public async Task<List<SupplierFAQ>> CreateAsync(List<SupplierFAQ> supplierFAQs)
        {
            _dbcontext.SupplierFAQs.AddRange(supplierFAQs);
            await _dbcontext.SaveChangesAsync();
            return supplierFAQs;
        }

        public async Task<List<SupplierFAQ>> UpdateAsync(List<SupplierFAQ> supplierFAQs)
        {
            _dbcontext.ChangeTracker.Clear(); 

            foreach (var supplierFAQ in supplierFAQs)
            {
                _dbcontext.SupplierFAQs.Update(supplierFAQ);
            }

            await _dbcontext.SaveChangesAsync();
            return supplierFAQs;
        }


        public async Task DeleteAsync(SupplierFAQ supplierFAQ)
        {
            _dbcontext.SupplierFAQs.Remove(supplierFAQ);
            await _dbcontext.SaveChangesAsync();
        }
    }
}
