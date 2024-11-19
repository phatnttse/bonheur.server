using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IUserRoleService
    {
        Task<ApplicationResponse> GetRoleByIdAsync(string roleId);
        Task<ApplicationResponse> GetRoleByNameAsync(string roleName);
        Task<ApplicationResponse> GetRoleLoadRelatedAsync(string roleName);
        Task<ApplicationResponse> GetRolesLoadRelatedAsync(int page, int pageSize);
        Task<ApplicationResponse> GetAllPermissions();
        Task<ApplicationResponse> CreateRoleAsync(UserRoleDTO createUserRoleDTO,
           IEnumerable<string> claims);
        Task<ApplicationResponse> UpdateRoleAsync(string id, UserRoleDTO userRoleDTO,
            IEnumerable<string>? claims);
        Task<ApplicationResponse> DeleteRoleAsync(string id);
    }
}
