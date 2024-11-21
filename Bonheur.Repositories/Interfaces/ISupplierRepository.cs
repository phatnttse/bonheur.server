using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories.Interfaces
{
    public interface ISupplierRepository
    {
        Task<Supplier?> GetSupplierByIdAsync(int id);
        Task<Supplier?> GetSupplierByUserIdAsync(string userId);
        Task<IPagedList<Supplier>> GetSuppliersAsync(
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
            );
        Task<Supplier?> CreateSupplierAsync(Supplier supplier);
        Task<Supplier?> UpdateSupplierAsync(Supplier supplier);
        Task<bool> DeleteSupplierAsync(int id);

    }
}
