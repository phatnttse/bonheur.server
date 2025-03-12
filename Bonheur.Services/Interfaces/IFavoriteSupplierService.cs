using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList.Extensions;
using X.PagedList;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.FavoriteSupplier;

namespace Bonheur.Services.Interfaces
{
    public interface IFavoriteSupplierService
    {
        Task<ApplicationResponse> AddFavoriteSupplier(int supplierId);

        Task<ApplicationResponse> GetAllFavoriteSuppliers(int pageNumber, int pageSize);

        Task<ApplicationResponse> GetFavoriteSupplierAsync(int id);

        Task<ApplicationResponse> GetFavoriteSuppliersByCategoryId(int categoryId, int pageNumber, int pageSize);

        Task<ApplicationResponse> DeleteSupplierAsync(int id);

        Task<ApplicationResponse> GetFavoriteSupplierCountByCategoryAsync();
    }
}
