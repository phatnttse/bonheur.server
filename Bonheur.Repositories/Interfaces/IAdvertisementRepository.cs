using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface IAdvertisementRepository
    {
        Task<List<Advertisement>> GetAdvertisementsAsync();
        Task<Advertisement?> GetAdvertisementByIdAsync(int id);
        Task AddAdvertisementAsync(Advertisement advertisement);
        Task UpdateAdvertisementAsync(Advertisement advertisement);
        Task DeleteAdvertisementAsync(Advertisement advertisement);
    }
}
