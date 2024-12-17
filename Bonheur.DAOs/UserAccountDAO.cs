using Bonheur.BusinessObjects.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Bonheur.DAOs
{
    public class UserAccountDAO
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAccountDAO(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser?> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<(ApplicationUser User, string[] Roles)?> GetUserAndRolesAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();

            if (user == null)
                return null;

            var userRoleIds = user.Roles.Select(r => r.RoleId).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name!)
                .ToArrayAsync();

            return (user, roles);
        }

        public async Task<IPagedList<(ApplicationUser User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize, string? search, string? role)
        {
            IQueryable<ApplicationUser> usersQuery = _context.Users
                .Include(u => u.Roles) 
                .OrderBy(u => u.UserName);

            if (!string.IsNullOrEmpty(search))
            {
                usersQuery = usersQuery.Where(u => u.UserName!.Contains(search) || u.FullName!.Contains(search));
            }

            if (!string.IsNullOrEmpty(role))
            {
                var roleEntity = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == role);

                if (roleEntity != null)
                {
                    usersQuery = usersQuery.Where(u => u.Roles.Any(ur => ur.RoleId == roleEntity.Id));
                }
            }

            var usersPagedList = usersQuery.ToPagedList(page, pageSize);
            var users = usersPagedList.ToList();

            var userRoleIds = users.SelectMany(u => u.Roles.Select(ur => ur.RoleId)).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .ToArrayAsync();

            var usersWithRoles = users
                .Select(u => (
                    u,
                    u.Roles.Select(ur => roles.FirstOrDefault(r => r.Id == ur.RoleId)?.Name).ToArray()
                ))
                .ToList();

            var result = new StaticPagedList<(ApplicationUser User, string[] Roles)>(
                usersWithRoles!,
                usersPagedList.PageNumber,
                usersPagedList.PageSize,
                usersPagedList.TotalItemCount);

            return result;
        }



        public async Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user,
            IEnumerable<string> roles, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            user = (await _userManager.FindByNameAsync(user.UserName!))!;

            try
            {
                result = await _userManager.AddToRolesAsync(user, roles.Distinct());
            }
            catch
            {
                await DeleteUserAsync(user);
                throw;
            }

            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return (false, result.Errors.Select(e => e.Description).ToArray());
            }

            return (true, []);
        }

        public async Task<(bool Succeeded, string[] Errors)> CreateUserNotPassword(ApplicationUser user,
           IEnumerable<string> roles)
        {
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            user = (await _userManager.FindByNameAsync(user.UserName!))!;

            try
            {
                result = await _userManager.AddToRolesAsync(user, roles.Distinct());
            }
            catch
            {
                await DeleteUserAsync(user);
                throw;
            }

            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return (false, result.Errors.Select(e => e.Description).ToArray());
            }

            return (true, []);
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user)
        {
            return await UpdateUserAsync(user, null);
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user,
            IEnumerable<string>? roles)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            if (roles != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var rolesToRemove = userRoles.Except(roles).ToArray();
                var rolesToAdd = roles.Except(userRoles).Distinct().ToArray();

                if (rolesToRemove.Length != 0)
                {
                    result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }

                if (rolesToAdd.Length != 0)
                {
                    result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return (true, []);
        }

        public async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(ApplicationUser user, string resetToken,
            string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdatePasswordAsync(ApplicationUser user,
            string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            return (true, []);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                if (!_userManager.SupportsUserLockout)
                    await _userManager.AccessFailedAsync(user);

                return false;
            }

            return true;
        }

        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
                return await DeleteUserAsync(user);

            return (true, []);
        }

        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }


        /// <summary>
        /// Lấy thông tin login của người dùng dựa trên provider và provider key.
        /// </summary>
        public async Task<IdentityUserLogin<string>?> GetUserLoginAsync(string loginProvider, string providerKey)
        {
            return await _context.UserLogins
                .Where(login => login.LoginProvider == loginProvider && login.ProviderKey == providerKey)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Thêm thông tin đăng nhập cho người dùng.
        /// </summary>
        public async Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo loginInfo)
        {
            return await _userManager.AddLoginAsync(user, loginInfo);
        }

        public async Task<string> GenereEmailConfirmationTokenAsync(ApplicationUser user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

    }
}
