using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Advertisement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IAdvertisementService
    {
        Task<ApplicationResponse> GetAdvertisementsAsync(string? searchTitle, string? searchContent, int pageNumber, int pageSize);
        Task<ApplicationResponse> GetAdvertisementByIdAsync(int id);
        Task<ApplicationResponse> AddAdvertisementAsync(AdvertisementDTO advertisementDTO);
        Task<ApplicationResponse> UpdateAdvertisementAsync(int id, AdvertisementDTO advertisementDTO);
        Task<ApplicationResponse> DeleteAdvertisementAsync(int id);
    }
}
