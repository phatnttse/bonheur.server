using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.SubscriptionPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface ISubscriptionPackageService
    {
        Task<ApplicationResponse> GetSubscriptionPackageByIdAsync(int id);
        Task<ApplicationResponse> GetAllSubscriptionPackagesAsync();
        Task<ApplicationResponse> CreateSubscriptionPackageAsync(SubscriptionPackageDTO subscriptionPackageDTO);
        Task<ApplicationResponse> UpdateSubscriptionPackageAsync(int id, SubscriptionPackageDTO subscriptionPackageDTO);
        Task<ApplicationResponse> DeleteSubscriptionPackageAsync(int id);
    }
}
