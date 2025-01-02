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
    public class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly AdvertisementDAO _advertisementDAO;

        public AdvertisementRepository(AdvertisementDAO advertisementDAO)
        {
            _advertisementDAO = advertisementDAO;
        }

        public async Task AddAdvertisementAsync(Advertisement advertisement) => await _advertisementDAO.AddAdvertisement(advertisement);
        public async Task<List<Advertisement>> GetAdvertisementsAsync() => await _advertisementDAO.GetAdvertisements();
        public async Task<Advertisement?> GetAdvertisementByIdAsync(int id) => await _advertisementDAO.GetAdvertisementById(id);
        public async Task UpdateAdvertisementAsync(Advertisement advertisement) => await _advertisementDAO.UpdateAdvertisement(advertisement);
        public async Task DeleteAdvertisementAsync(Advertisement advertisement) => await _advertisementDAO.DeleteAdvertisement(advertisement);

    }
}
