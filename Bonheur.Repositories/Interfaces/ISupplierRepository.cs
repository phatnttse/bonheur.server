using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
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
        Task<Supplier?> GetSupplierByIdAsync(int id, bool isIncludeUser);
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
                string? orderBy,
                int pageNumber = 1,
                int pageSize = 10
            );
        Task<Supplier?> CreateSupplierAsync(Supplier supplier);
        Task<Supplier?> UpdateSupplierAsync(Supplier supplier);
        Task<bool> DeleteSupplierAsync(int id);
        Task<bool> IsSupplierAsync(string userId);
        Task<Supplier?> GetSupplierBySlugAsync(string slug);
        Task<List<Supplier>> GetAllSuppliersAsync();
        Task<IPagedList<Supplier>> GetSuppliersByAdminAsync(
               string? supplierName,
               int? supplierCategoryId,
               string? province,
               bool? isFeatured,
               decimal? averageRating,
               decimal? minPrice,
               decimal? maxPrice,
               SupplierStatus? status,
               bool? sortAsc,
               string? orderBy,
               int pageNumber = 1,
               int pageSize = 10
           );

    }
}
