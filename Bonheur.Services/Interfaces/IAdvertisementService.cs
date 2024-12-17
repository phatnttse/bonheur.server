using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IAdvertisementService
    {
        Task<ApplicationResponse> GetAdvertisementsAsync();
        Task<ApplicationResponse> GetAdvertisementByIdAsync(int id);
        Task<ApplicationResponse> AddAdvertisementAsync(Advertisement advertisement);
        Task<ApplicationResponse> UpdateAdvertisementAsync(Advertisement advertisement);
        Task<ApplicationResponse> DeleteAdvertisementAsync(Advertisement advertisement);
    }
}
