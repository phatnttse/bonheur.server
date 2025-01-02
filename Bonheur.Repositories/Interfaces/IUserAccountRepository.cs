using Bonheur.BusinessObjects.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories.Interfaces
{
    public interface IUserAccountRepository
    {
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByUserNameAsync(string userName);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<(ApplicationUser User, string[] Roles)?> GetUserAndRolesAsync(string userId);
        Task<IPagedList<(ApplicationUser User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize, string? search, string? role);
        Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user,
            IEnumerable<string> roles, string password);
        Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user);
        Task<(bool Succeeded, string[] Errors)> UpdateUserAndUserRoleAsync(ApplicationUser user,
            IEnumerable<string>? roles);
        Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(ApplicationUser user, string resetToken,
            string newPassword);
        Task<(bool Succeeded, string[] Errors)> UpdatePasswordAsync(ApplicationUser user,
            string currentPassword, string newPassword);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string userId);
        Task<IdentityUserLogin<string>?> GetUserLoginAsync(string loginProvider, string providerKey);
        Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo loginInfo);
        Task<(bool Succeeded, string[] Errors)> CreateUserNotPassword(ApplicationUser user,
           IEnumerable<string> roles);
        Task<string> GenereEmailConfirmationTokenAsync(ApplicationUser user);
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles);
    }
}
