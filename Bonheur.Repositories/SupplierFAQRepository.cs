using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories
{
    public class SupplierFAQRepository : ISupplierFAQRepository
    {
        private readonly SupplierFAQDAO _dao;

        public SupplierFAQRepository(SupplierFAQDAO dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<SupplierFAQ>> GetAllAsync() => await _dao.GetAllAsync();
        public async Task<SupplierFAQ?> GetByIdAsync(int id) => await _dao.GetByIdAsync(id);
        public async Task<List<SupplierFAQ>> GetByIdsAsync(List<int> ids) => await _dao.GetByIdsAsync(ids);
        public async Task<IEnumerable<SupplierFAQ>> GetSupplierFAQsBySupplierIdAsync(int supplierId) => await _dao.GetSupplierFAQsBySupplierIdAsync(supplierId);
        public async Task<List<SupplierFAQ>> CreateAsync(List<SupplierFAQ> supplierFAQs) => await _dao.CreateAsync(supplierFAQs);
        public async Task<List<SupplierFAQ>> UpdateAsync(List<SupplierFAQ> supplierFAQs) => await _dao.UpdateAsync(supplierFAQs);
        public async Task DeleteAsync(SupplierFAQ supplierFAQ) => await _dao.DeleteAsync(supplierFAQ);
    }
}
