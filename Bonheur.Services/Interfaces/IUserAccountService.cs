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
    public interface IUserAccountService
    {
        Task<ApplicationResponse> GetCurrentUserAsync();
        Task<ApplicationUser?> GetCurrentUser();
        Task<ApplicationResponse> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationResponse> GetUserRolesAsync(ApplicationUser user);
        Task<ApplicationResponse> GetUserAndRolesAsync(string userId);
        Task<ApplicationResponse> GetUsersAndRolesAsync(int page, int pageSize);
        Task<ApplicationResponse> CreateUserAsync(UserAccountDTO user,
            IEnumerable<string> roles, string password);
        Task<ApplicationResponse> UpdateCurrentUserAsync(UpdateUserProfileDTO updateUserProfile);
        Task<ApplicationResponse> UpdateUserAndUserRoleAsync(string id, UserAccountDTO userAccountDTO);
        Task<ApplicationResponse> UpdateUserAccountStatusAsync(string id, UserAccountStatusDTO userAccountDTO);
        Task<ApplicationResponse> UpdatePasswordAsync(string currentPassword, string newPassword);
        Task<ApplicationResponse> DeleteUserAsync(string userId);
    }
}
