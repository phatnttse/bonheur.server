using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Account;
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

                var userAndRole = await _userAccountRepository.GetUserAndRolesAsync(currentUserId);

                if (userAndRole == null) throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                var userData = _mapper.Map<UserAccountDTO>(userAndRole.Value.User);
                userData.Roles = userAndRole.Value.Roles;

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

                var userData = _mapper.Map<UserAccountDTO>(userAndRoles.Value.User);

                userData.Roles = userAndRoles.Value.Roles;

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Get user and roles by id successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = userData
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

                var userAccountDTOs = new List<UserAccountDTO>();

                foreach (var item in usersAndRoles)
                {
                    var userAccountDTO = _mapper.Map<UserAccountDTO>(item.User);
                    userAccountDTO.Roles = item.Roles;

                    userAccountDTOs.Add(userAccountDTO);
                }

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Get users and roles successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = userAccountDTOs
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

        public async Task<ApplicationResponse> CreateUserAsync(UserAccountDTO userAccountDTO, IEnumerable<string> roles, string password)
        {
            try
            {
                var applicationUser = _mapper.Map<ApplicationUser>(userAccountDTO);

                var result = await _userAccountRepository.CreateUserAsync(applicationUser, roles, password);

                if (!result.Succeeded) throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);

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

                if (!result.Succeeded) throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);

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

        public async Task<ApplicationResponse> UpdatePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                var existingUser = await GetCurrentUser();

                var result = await _userAccountRepository.UpdatePasswordAsync(existingUser!, currentPassword, newPassword);

                if (!result.Succeeded) throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);

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

        public async Task<ApplicationResponse> UpdateUserAndUserRoleAsync(string id, UserAccountDTO userAccountDTO)
        {
            try
            {
                var existingUser = await _userAccountRepository.GetUserByIdAsync(id);

                _mapper.Map(userAccountDTO, existingUser);

                var result = await _userAccountRepository.UpdateUserAndUserRoleAsync(existingUser!, userAccountDTO.Roles);

                if (!result.Succeeded) throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "User updated successfully",
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

        public async Task<ApplicationUser?> GetCurrentUser()
        {       
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                var user = await _userAccountRepository.GetUserByIdAsync(currentUserId);

                if (user == null) throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

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

        public async Task<ApplicationResponse> UpdateCurrentUserAsync(UpdateUserProfileDTO updateUserProfileDTO)
        {
            try
            {
                var existingUser = await GetCurrentUser();
          
                _mapper.Map(updateUserProfileDTO, existingUser);

                var result = await _userAccountRepository.UpdateUserAsync(existingUser!);

                if (!result.Succeeded) throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);

                var userUpdated = _mapper.Map<UserAccountDTO>(existingUser);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Profile updated successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = userUpdated
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

        public async Task<ApplicationResponse> UpdateUserAccountStatusAsync(string id, UserAccountStatusDTO userAccountStatusDTO)
        {
            try
            {
                var existingUser = await _userAccountRepository.GetUserByIdAsync(id);

                _mapper.Map(userAccountStatusDTO, existingUser);

                var result = await _userAccountRepository.UpdateUserAsync(existingUser!);

                if (!result.Succeeded) throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "User updated successfully",
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
