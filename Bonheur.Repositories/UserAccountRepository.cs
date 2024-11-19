﻿using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly UserAccountDAO _userAccountDAO;
        public UserAccountRepository(UserAccountDAO userAccountDAO)
        {
            _userAccountDAO = userAccountDAO;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId) => await _userAccountDAO.GetUserByIdAsync(userId);
        public async Task<ApplicationUser?> GetUserByUserNameAsync(string userName) => await _userAccountDAO.GetUserByUserNameAsync(userName);
        public async Task<ApplicationUser?> GetUserByEmailAsync(string email) => await _userAccountDAO.GetUserByEmailAsync(email);
        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user) => await _userAccountDAO.GetUserRolesAsync(user);
        public async Task<(ApplicationUser User, string[] Roles)?> GetUserAndRolesAsync(string userId) => await _userAccountDAO.GetUserAndRolesAsync(userId);
        public async Task<List<(ApplicationUser User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize) => await _userAccountDAO.GetUsersAndRolesAsync(page, pageSize);
        public async Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user,
            IEnumerable<string> roles, string password) => await _userAccountDAO.CreateUserAsync(user, roles, password);
        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user) => await _userAccountDAO.UpdateUserAsync(user);
        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAndUserRoleAsync(ApplicationUser user,
            IEnumerable<string>? roles) => await _userAccountDAO.UpdateUserAsync(user, roles);
        public async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(ApplicationUser user,
            string newPassword) => await _userAccountDAO.ResetPasswordAsync(user, newPassword);
        public async Task<(bool Succeeded, string[] Errors)> UpdatePasswordAsync(ApplicationUser user,
            string currentPassword, string newPassword) => await _userAccountDAO.UpdatePasswordAsync(user, currentPassword, newPassword);
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password) => await _userAccountDAO.CheckPasswordAsync(user, password);
        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string userId) => await _userAccountDAO.DeleteUserAsync(userId);
    }
}
