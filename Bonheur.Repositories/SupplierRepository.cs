using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly SupplierDAO _supplierDAO;

        public SupplierRepository(SupplierDAO supplierDAO)
        {
            _supplierDAO = supplierDAO;
        }

        public Task<Supplier?> CreateSupplierAsync(Supplier supplier) => _supplierDAO.CreateSupplierAsync(supplier);

        public Task<bool> DeleteSupplierAsync(int id) => _supplierDAO.DeleteSupplierAsync(id);

        public Task<Supplier?> GetSupplierByIdAsync(int id) => _supplierDAO.GetSupplierByIdAsync(id);

        public Task<Supplier?> GetSupplierByUserIdAsync(string userId) => _supplierDAO.GetSupplierByUserIdAsync(userId);

        public Task<IPagedList<Supplier>> GetSuppliersAsync(
                string? supplierName,
                int? supplierCategoryId,
                string? province,
                bool? isFeatured,
                decimal? averageRating,
                decimal? minPrice,
                decimal? maxPrice,
                bool? sortAsc,
                int pageNumber = 1,
                int pageSize = 10
            ) => _supplierDAO.GetSuppliersAsync(supplierName, supplierCategoryId, province, isFeatured, averageRating, minPrice, maxPrice, sortAsc, pageNumber, pageSize);

        public Task<Supplier?> UpdateSupplierAsync(Supplier supplier) => _supplierDAO.UpdateSupplierAsync(supplier);
    }
}
