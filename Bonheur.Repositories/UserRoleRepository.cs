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
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly UserRoleDAO _userRoleDAO;

        public UserRoleRepository(UserRoleDAO userRoleDAO)
        {
            _userRoleDAO = userRoleDAO;
        }
        public async Task<ApplicationRole?> GetRoleByIdAsync(string roleId) => await _userRoleDAO.GetRoleByIdAsync(roleId);
        public async Task<ApplicationRole?> GetRoleByNameAsync(string roleName) => await _userRoleDAO.GetRoleByNameAsync(roleName);
        public async Task<ApplicationRole?> GetRoleLoadRelatedAsync(string roleName) => await _userRoleDAO.GetRoleLoadRelatedAsync(roleName);
        public async Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize) => await _userRoleDAO.GetRolesLoadRelatedAsync(page, pageSize);
        public async Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(ApplicationRole role,
           IEnumerable<string> claims) => await _userRoleDAO.CreateRoleAsync(role, claims);
        public async Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(ApplicationRole role,
            IEnumerable<string>? claims) => await _userRoleDAO.UpdateRoleAsync(role, claims);
        public async Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(ApplicationRole role) => await _userRoleDAO.DeleteRoleAsync(role);
    }
}
