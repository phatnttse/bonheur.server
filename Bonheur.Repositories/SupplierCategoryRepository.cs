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

        public async Task<bool> AddSupplierCategory(SupplierCategory supplierCategory) => await _supplierCategoriesDAO.CreateNewSupplierCategory(supplierCategory);

        public async Task<bool> DeleteSupplierCategory(int id) => await _supplierCategoriesDAO.DeleteSupplierCategory(id);

        public async Task<List<SupplierCategory>> GetAllSupplierCategoryAsync() => await _supplierCategoriesDAO.GetSupplierCategoriesAsync();

        public async Task<SupplierCategory> GetSupplierCategoryByIdAsync(int id) => await _supplierCategoriesDAO.GetSupplierCategoryById(id);

        public async Task<bool> UpdateSupplierCategory(SupplierCategory supplierCategory) => await _supplierCategoriesDAO.UpdateSupplierCategory(supplierCategory);
    }
}
