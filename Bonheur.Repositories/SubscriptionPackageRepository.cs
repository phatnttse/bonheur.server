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
    public class SubscriptionPackageRepository : ISubscriptionPackageRepository
    {
        private readonly SubscriptionPackageDAO _subscriptionPackageDAO;

        public SubscriptionPackageRepository(SubscriptionPackageDAO subscriptionPackageDAO)
        {
            _subscriptionPackageDAO = subscriptionPackageDAO;
        }

        public async Task<SubscriptionPackage?> GetSubscriptionPackageByIdAsync(int id) => await _subscriptionPackageDAO.GetSubscriptionPackageByIdAsync(id);
        public async Task<List<SubscriptionPackage>> GetAllSubscriptionPackagesAsync() => await _subscriptionPackageDAO.GetAllSubscriptionPackagesAsync();
        public async Task CreateSubscriptionPackageAsync(SubscriptionPackage subscriptionPackage) => await _subscriptionPackageDAO.CreateSubscriptionPackageAsync(subscriptionPackage);
        public async Task UpdateSubscriptionPackageAsync(SubscriptionPackage subscriptionPackage) => await _subscriptionPackageDAO.UpdateSubscriptionPackageAsync(subscriptionPackage);
        public async Task DeleteSubscriptionPackageAsync(SubscriptionPackage subscriptionPackage) => await _subscriptionPackageDAO.DeleteSubscriptionPackageAsync(subscriptionPackage);
    }
}
