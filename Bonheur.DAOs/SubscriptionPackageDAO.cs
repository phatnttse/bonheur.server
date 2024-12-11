using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.DAOs
{
    public class SubscriptionPackageDAO
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionPackageDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SubscriptionPackage?> GetSubscriptionPackageByIdAsync(int id)
        {
            return await _context.SubscriptionPackages.FindAsync(id);
        }

        public async Task<List<SubscriptionPackage>> GetAllSubscriptionPackagesAsync()
        {
            return await _context.SubscriptionPackages.ToListAsync();
        }

        public async Task CreateSubscriptionPackageAsync(SubscriptionPackage subscriptionPackage)
        {
            _context.SubscriptionPackages.Add(subscriptionPackage);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSubscriptionPackageAsync(SubscriptionPackage subscriptionPackage)
        {
            _context.SubscriptionPackages.Update(subscriptionPackage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubscriptionPackageAsync(SubscriptionPackage subscriptionPackage)
        {        
            _context.SubscriptionPackages.Remove(subscriptionPackage);
            await _context.SaveChangesAsync();
        }
    }
}
