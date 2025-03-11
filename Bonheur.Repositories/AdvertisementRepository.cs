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
    public class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly AdvertisementDAO _advertisementDAO;

        public AdvertisementRepository(AdvertisementDAO advertisementDAO)
        {
            _advertisementDAO = advertisementDAO;
        }

        public async Task AddAdvertisementAsync(Advertisement advertisement) => await _advertisementDAO.AddAdvertisement(advertisement);
        public async Task<IPagedList<Advertisement>> GetAdvertisementsAsync(string? searchTitle, string? searchContent, int pageNumber, int pageSize) => await _advertisementDAO.GetAdvertisements(searchTitle, searchContent, pageNumber, pageSize);
        public async Task<Advertisement?> GetAdvertisementByIdAsync(int id) => await _advertisementDAO.GetAdvertisementById(id);
        public async Task UpdateAdvertisementAsync(Advertisement advertisement) => await _advertisementDAO.UpdateAdvertisement(advertisement);
        public async Task DeleteAdvertisementAsync(Advertisement advertisement) => await _advertisementDAO.DeleteAdvertisement(advertisement);
        public async Task<int> GetTotalAdvertisementsCountAsync() => await _advertisementDAO.GetTotalAdvertisementsCount();
        public async Task<IPagedList<Advertisement>> GetAdvertisementsBySupplier(int supplierId, int pageNumber = 1, int pageSize = 10) => await _advertisementDAO.GetAdvertisementsBySupplier(supplierId, pageNumber, pageSize);
    }
}
