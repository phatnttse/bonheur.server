using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.UserRole;
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
        Task<ApplicationResponse> CreateRoleAsync(CreateUserRoleDTO createUserRoleDTO,
           IEnumerable<string> claims);
        Task<ApplicationResponse> UpdateRoleAsync(ApplicationRole role,
            IEnumerable<string>? claims);
        Task<ApplicationResponse> DeleteRoleAsync(string roleName);
    }
}
