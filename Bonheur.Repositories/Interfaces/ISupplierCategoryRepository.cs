using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface ISupplierCategoryRepository
    {
        Task<SupplierCategory> GetSupplierCategoryByIdAsync(int id);
        Task<List<SupplierCategory>> GetAllSupplierCategoryAsync();
        Task AddSupplierCategory(SupplierCategory supplierCategory);
        Task UpdateSupplierCategory(SupplierCategory supplierCategory);
        Task DeleteSupplierCategory(int id);
    }
}
