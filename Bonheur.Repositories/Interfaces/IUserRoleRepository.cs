using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<ApplicationRole?> GetRoleByIdAsync(string roleId);
        Task<ApplicationRole?> GetRoleByNameAsync(string roleName);
        Task<ApplicationRole?> GetRoleLoadRelatedAsync(string roleName);
        Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize);
        Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(ApplicationRole role,
           IEnumerable<string> claims);

        Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(ApplicationRole role,
            IEnumerable<string>? claims);

        Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(ApplicationRole role);
    }
}
