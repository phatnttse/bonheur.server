using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.DAOs
{
    public class SocialNetworkDAO
    {
        private readonly ApplicationDbContext _dbcontext;

        public SocialNetworkDAO(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<IEnumerable<SocialNetwork>> GetAllAsync()
        {
            return await _dbcontext.SocialNetworks.ToListAsync();
        }

        public async Task<SocialNetwork?> GetByIdAsync(int id)
        {
            return await _dbcontext.SocialNetworks.FindAsync(id);
        }

        public async Task<List<SocialNetwork>> GetByIdsAsync(List<int> ids)
        {
            return await _dbcontext.SocialNetworks
                .Where(sn => ids.Contains(sn.Id))
                .ToListAsync();
        }

        public async Task<SocialNetwork> CreateAsync(SocialNetwork socialNetwork)
        {
            _dbcontext.SocialNetworks.Add(socialNetwork);
            await _dbcontext.SaveChangesAsync();
            return socialNetwork;
        }

        public async Task<SocialNetwork> UpdateAsync(SocialNetwork socialNetwork)
        {
            _dbcontext.SocialNetworks.Update(socialNetwork);
            await _dbcontext.SaveChangesAsync();
            return socialNetwork;
        }

        public async Task DeleteAsync(SocialNetwork socialNetwork)
        {         
             _dbcontext.SocialNetworks.Remove(socialNetwork);
             await _dbcontext.SaveChangesAsync();           
        }
    }
}
