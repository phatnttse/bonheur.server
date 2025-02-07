using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface ISocialNetworkRepository
    {
        Task<IEnumerable<SocialNetwork>> GetAllAsync();
        Task<SocialNetwork?> GetByIdAsync(int id);
        Task<List<SocialNetwork>> GetByIdsAsync(List<int> ids);
        Task<SocialNetwork> CreateAsync(SocialNetwork socialNetwork);
        Task<SocialNetwork> UpdateAsync(SocialNetwork socialNetwork);
        Task DeleteAsync(SocialNetwork socialNetwork);
    }
}
