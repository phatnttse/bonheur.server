using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IUserAccountService
    {
        Task<ApplicationResponse> GetCurrentUserAsync();
        Task<ApplicationResponse> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationResponse> GetUserRolesAsync(ApplicationUser user);
        Task<ApplicationResponse> GetUserAndRolesAsync(string userId);
        Task<ApplicationResponse> GetUsersAndRolesAsync(int page, int pageSize);
        Task<ApplicationResponse> CreateUserAsync(ApplicationUser user,
            IEnumerable<string> roles, string password);
        Task<ApplicationResponse> UpdateUserAsync(ApplicationUser user);
        Task<ApplicationResponse> UpdateUserAndUserRoleAsync(ApplicationUser user,
            IEnumerable<string>? roles);
        Task<ApplicationResponse> ResetPasswordAsync(ApplicationUser user,
            string newPassword);
        Task<ApplicationResponse> UpdatePasswordAsync(ApplicationUser user,
            string currentPassword, string newPassword);
        Task<ApplicationResponse> CheckPasswordAsync(ApplicationUser user, string password);
        Task<ApplicationResponse> DeleteUserAsync(string userId);
    }
}
