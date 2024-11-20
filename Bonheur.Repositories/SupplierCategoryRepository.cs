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
    public class SupplierCategoryRepository : ISupplierCategoryRepository
    {
        private readonly SupplierCategoriesDAO _supplierCategoriesDAO;

        public SupplierCategoryRepository(SupplierCategoriesDAO supplierCategoriesDAO)
        {
            _supplierCategoriesDAO = supplierCategoriesDAO;
        }

        public Task AddSupplierCategory(SupplierCategory supplierCategory) => _supplierCategoriesDAO.CreateNewSupplierCategory(supplierCategory);

        public Task DeleteSupplierCategory(int id) => _supplierCategoriesDAO.DeleteSupplierCategory(id);

        public Task<List<SupplierCategory>> GetAllSupplierCategoryAsync() => _supplierCategoriesDAO.GetSupplierCategoriesAsync();

        public Task<SupplierCategory> GetSupplierCategoryByIdAsync(int id) => _supplierCategoriesDAO.GetSupplierCategoryById(id);

        public Task UpdateSupplierCategory(SupplierCategory supplierCategory) => _supplierCategoriesDAO.UpdateSupplierCategory(supplierCategory);
    }
}
