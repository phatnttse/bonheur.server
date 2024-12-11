using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface ISubscriptionPackageRepository
    {
        Task<SubscriptionPackage?> GetSubscriptionPackageByIdAsync(int id);
        Task<List<SubscriptionPackage>> GetAllSubscriptionPackagesAsync();
        Task CreateSubscriptionPackageAsync(SubscriptionPackage subscriptionPackage);
        Task UpdateSubscriptionPackageAsync(SubscriptionPackage subscriptionPackage);
        Task DeleteSubscriptionPackageAsync(SubscriptionPackage subscriptionPackage);
    }
}
