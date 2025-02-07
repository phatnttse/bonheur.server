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
    public class SocialNetworkRepository : ISocialNetworkRepository
    {
        private readonly SocialNetworkDAO _dao;

        public SocialNetworkRepository(SocialNetworkDAO dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<SocialNetwork>> GetAllAsync() => await _dao.GetAllAsync();
        public async Task<SocialNetwork?> GetByIdAsync(int id) => await _dao.GetByIdAsync(id);
        public async Task<List<SocialNetwork>> GetByIdsAsync(List<int> ids) => await _dao.GetByIdsAsync(ids);
        public async Task<SocialNetwork> CreateAsync(SocialNetwork socialNetwork) => await _dao.CreateAsync(socialNetwork);
        public async Task<SocialNetwork> UpdateAsync(SocialNetwork socialNetwork) => await _dao.UpdateAsync(socialNetwork);
        public async Task DeleteAsync(SocialNetwork socialNetwork) => await _dao.DeleteAsync(socialNetwork);
    }
}
