using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bonheur.DAOs
{
    public class SupplierSocialNetworkDAO
    {
        private readonly ApplicationDbContext _dbcontext;

        public SupplierSocialNetworkDAO(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<IEnumerable<SupplierSocialNetwork>> GetAllAsync()
        {
            return await _dbcontext.SupplierSocialNetworks.ToListAsync();
        }

        public async Task<SupplierSocialNetwork?> GetByIdAsync(int id)
        {
            return await _dbcontext.SupplierSocialNetworks.FindAsync(id);
        }

        public async Task<List<SupplierSocialNetwork>> GetSupplierSocialNetworksBySupplierIdAsync(int supplierId)
        {
            return await _dbcontext.SupplierSocialNetworks.Where(s => s.SupplierId == supplierId).ToListAsync();
        }

        public async Task<List<SupplierSocialNetwork>> GetBySocialNetworkIdsAndSupplierIdAsync(List<int> socialNetworkIds, int supplierId)
        {
            return await _dbcontext.SupplierSocialNetworks.Where(s => socialNetworkIds.Contains(s.SocialNetworkId) && s.SupplierId == supplierId).ToListAsync();
        }

        public async Task<List<SupplierSocialNetwork>> CreateAsync(List<SupplierSocialNetwork> supplierSocialNetworks)
        {
            await _dbcontext.SupplierSocialNetworks.AddRangeAsync(supplierSocialNetworks);
            await _dbcontext.SaveChangesAsync();
            return supplierSocialNetworks;
        }

        public async Task<List<SupplierSocialNetwork>> UpdateAsync(List<SupplierSocialNetwork> supplierSocialNetworks)
        {
            foreach (var socialNetwork in supplierSocialNetworks)
            {
                _dbcontext.SupplierSocialNetworks.Attach(socialNetwork);
                _dbcontext.Entry(socialNetwork).State = EntityState.Modified;
            }

            await _dbcontext.SaveChangesAsync();
            return supplierSocialNetworks;
        }

        public async Task DeleteAsync(SupplierSocialNetwork supplierSocialNetwork)
        {
            _dbcontext.SupplierSocialNetworks.Remove(supplierSocialNetwork);
            await _dbcontext.SaveChangesAsync();
        }
    }
}
