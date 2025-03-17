using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList.Extensions;
using X.PagedList;

namespace Bonheur.Repositories.Interfaces
{
    public interface IFavoriteSupplierRepository
    {
        Task<FavoriteSupplier> AddFavoriteSupplier(FavoriteSupplier favoriteSupplier);

        Task<IPagedList<FavoriteSupplier>> GetAllFavoriteSuppliers(string userId, int pageNumber, int pageSize);

        Task<IPagedList<FavoriteSupplier>> GetFavoriteSuppliersByCategoryId(string userId, int categoryId, int pageNumber, int pageSize);


        Task<FavoriteSupplier?> GetFavoriteSupplierAsync(int id);

        Task<FavoriteSupplier> DeleteSupplierAsync(FavoriteSupplier favoriteSupplier);

        Task<object> GetFavoriteSupplierCountByCategoryAsync();

        Task<bool> IsFavoriteSupplierAsync(string userId, int supplierId); 
    }
}
