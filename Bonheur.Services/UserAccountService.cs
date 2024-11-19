using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.UserAccount;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;

namespace Bonheur.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IMapper _mapper;

        public UserAccountService(IUserAccountRepository userAccountRepository, IMapper mapper)
        {
            _userAccountRepository = userAccountRepository;
            _mapper = mapper;
        }

        public async Task<ApplicationResponse> GetCurrentUserAsync()
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized); 

                var user = await _userAccountRepository.GetUserByIdAsync(currentUserId);

                if (user == null) throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                var userData = _mapper.Map<UserAccountDTO>(user);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Get current user successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = userData,
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userAccountRepository.GetUserByIdAsync(userId);

                if (user == null) throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                var userData = _mapper.Map<UserAccountDTO>(user);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Get user by id successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = userData,
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public async Task<ApplicationResponse> GetUserAndRolesAsync(string userId)
        {
            try
            {
                var userAndRoles = await _userAccountRepository.GetUserAndRolesAsync(userId);

                if (userAndRoles == null) throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Get user and roles by id successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = userAndRoles
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userAccountRepository.GetUserByEmailAsync(email);

                if (user == null) throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                return user;

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetUserRolesAsync(ApplicationUser user)
        {
           try
            {
                var roles = await _userAccountRepository.GetUserRolesAsync(user);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Get user roles successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = roles
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetUsersAndRolesAsync(int page, int pageSize)
        {
           try
            {
                var usersAndRoles = await _userAccountRepository.GetUsersAndRolesAsync(page, pageSize);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Get users and roles successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = usersAndRoles

                };

           }
           catch (ApiException)
           {
                throw;
           }
           catch (Exception ex)
           {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
           }
        }


        public async Task<ApplicationResponse> CheckPasswordAsync(ApplicationUser user, string password)
        {
            try
            {
                var result = await _userAccountRepository.CheckPasswordAsync(user, password);

                if (!result) throw new ApiException("Password is incorrect", System.Net.HttpStatusCode.BadRequest);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Password is correct",
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> CreateUserAsync(ApplicationUser user, IEnumerable<string> roles, string password)
        {
            try
            {
                var result = await _userAccountRepository.CreateUserAsync(user, roles, password);

                if (!result.Succeeded) throw new ApiException("Failed to create user", System.Net.HttpStatusCode.BadRequest);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "User created successfully",
                    StatusCode = System.Net.HttpStatusCode.Created
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> DeleteUserAsync(string userId)
        {
            try
            {
                var result = await _userAccountRepository.DeleteUserAsync(userId);

                if (!result.Succeeded) throw new ApiException("Failed to delete user", System.Net.HttpStatusCode.BadRequest);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "User deleted successfully",
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            try
            {
                var result = await _userAccountRepository.ResetPasswordAsync(user, newPassword);

                if (!result.Succeeded) throw new ApiException("Failed to reset password", System.Net.HttpStatusCode.BadRequest);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Password reset successfully",
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            try
            {
                var result = await _userAccountRepository.UpdatePasswordAsync(user, currentPassword, newPassword);

                if (!result.Succeeded) throw new ApiException("Failed to update password", System.Net.HttpStatusCode.BadRequest);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Password updated successfully",
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> UpdateUserAndUserRoleAsync(ApplicationUser user, IEnumerable<string>? roles)
        {
            try
            {
                var result = await _userAccountRepository.UpdateUserAndUserRoleAsync(user, roles);

                if (!result.Succeeded) throw new ApiException("Failed to update user and roles", System.Net.HttpStatusCode.BadRequest);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "User and roles updated successfully",
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                var result = await _userAccountRepository.UpdateUserAsync(user);

                if (!result.Succeeded) throw new ApiException("Failed to update user and roles", System.Net.HttpStatusCode.BadRequest);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "User and roles updated successfully",
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
