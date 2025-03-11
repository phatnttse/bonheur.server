using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories.Interfaces
{
    public interface IAdvertisementRepository
    {
        Task<IPagedList<Advertisement>> GetAdvertisementsAsync(string? searchTitle, string? searchContent, int pageNumber = 1, int pageSize = 10);
        Task<Advertisement?> GetAdvertisementByIdAsync(int id);
        Task AddAdvertisementAsync(Advertisement advertisement);
        Task UpdateAdvertisementAsync(Advertisement advertisement);
        Task DeleteAdvertisementAsync(Advertisement advertisement);
        Task<int> GetTotalAdvertisementsCountAsync();
        Task<IPagedList<Advertisement>> GetAdvertisementsBySupplier(int supplierId, int pageNumber = 1, int pageSize = 10);
    }
}
