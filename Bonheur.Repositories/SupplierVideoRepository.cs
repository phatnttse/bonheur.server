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
    public class SupplierVideoRepository : ISupplierVideoRepository
    {
        private readonly SupplierVideoDAO _supplierVideoDAO;

        public SupplierVideoRepository(SupplierVideoDAO supplierVideoDAO)
        {
            _supplierVideoDAO = supplierVideoDAO;
        }

        public async Task AddSupplierVideoAsync(SupplierVideo supplierVideo) => await _supplierVideoDAO.AddSupplierVideoAsync(supplierVideo);
        public async Task AddSupplierVideoRangeAsync(IEnumerable<SupplierVideo> supplierVideos) => await _supplierVideoDAO.AddSupplierVideoRangeAsync(supplierVideos);
        public async Task DeleteSupplierVideoAsync(SupplierVideo supplierVideo) => await _supplierVideoDAO.DeleteSupplierVideoAsync(supplierVideo);
        public async Task DeleteSupplierVideosBySupplierIdAsync(int supplierId) => await _supplierVideoDAO.DeleteSupplierVideosBySupplierIdAsync(supplierId);
        public async Task<IEnumerable<SupplierVideo>> GetSupplierVideosBySupplierIdAsync(int supplierId) => await _supplierVideoDAO.GetSupplierVideosBySupplierIdAsync(supplierId);
        public async Task<SupplierVideo?> GetSupplierVideoById(int id) => await _supplierVideoDAO.GetSupplierVideoById(id);
    }
}
