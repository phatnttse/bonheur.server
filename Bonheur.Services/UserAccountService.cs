using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.DTOs.Storage;
using Bonheur.Services.DTOs.Supplier;
using Bonheur.Services.Email;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Bonheur.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AuthService> _logger;

        public UserAccountService(IUserAccountRepository userAccountRepository, IMapper mapper, IStorageService storageService, IEmailSender emailSender, ILogger<AuthService> logger)
        {
            _userAccountRepository = userAccountRepository;
            _mapper = mapper;
            _storageService = storageService;
            _emailSender = emailSender;
            _logger = logger;
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

        public async Task<ApplicationResponse> GetUsersAndRolesAsync(int page, int pageSize, string? search = null, string? role = null)
        {
            try
            {
                var usersAndRolesPagedList = await _userAccountRepository.GetUsersAndRolesAsync(page, pageSize, search, role);

                var userAccountDTOs = new List<UserAccountDTO>();

                foreach (var item in usersAndRolesPagedList)
                {
                    var userAccountDTO = _mapper.Map<UserAccountDTO>(item.User);
                    userAccountDTO.Roles = item.Roles;

                    userAccountDTOs.Add(userAccountDTO);
                }

                var responseData = new PagedData<UserAccountDTO>
                {
                    Items = userAccountDTOs,
                    PageNumber = usersAndRolesPagedList.PageNumber,
                    PageSize = usersAndRolesPagedList.PageSize,
                    TotalItemCount = usersAndRolesPagedList.TotalItemCount,
                    PageCount = usersAndRolesPagedList.PageCount,
                    IsFirstPage = usersAndRolesPagedList.IsFirstPage,
                    IsLastPage = usersAndRolesPagedList.IsLastPage,
                    HasNextPage = usersAndRolesPagedList.HasNextPage,
                    HasPreviousPage = usersAndRolesPagedList.HasPreviousPage
                };


                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Get users and roles successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = responseData
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

        public async Task<ApplicationResponse> UploadAvatar(IFormFile file)
        {        
            try
            {
                var existingUser = await GetCurrentUser();

                AzureBlobResponseDTO response = await _storageService.UploadAsync(file);

                if (response.Error) throw new ApiException(response.Status!, System.Net.HttpStatusCode.BadRequest);

                existingUser!.PictureUrl = response.Blob.Uri;
                existingUser.PictureFileName = response.Blob.Name;

                var result = await _userAccountRepository.UpdateUserAsync(existingUser!);

                if (!result.Succeeded) throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);

                if (!string.IsNullOrEmpty(existingUser.PictureFileName))
                {
                    await _storageService.DeleteAsync(existingUser.PictureFileName);
                }

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Avatar uploaded successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = existingUser
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

        public async Task<ApplicationResponse> SendChangeEmailAsync(ChangeEmailDTO changeEmailDTO)
        {
            try
            {
               var currentUser = await GetCurrentUser();

               if (changeEmailDTO.Email == currentUser!.Email) throw new ApiException("New email cannot be the same as the current email", System.Net.HttpStatusCode.BadRequest);

               var existingEmail = await _userAccountRepository.GetUserByEmailAsync(changeEmailDTO.Email);

               if (existingEmail != null) throw new ApiException("Email already exists", System.Net.HttpStatusCode.BadRequest);

               string changeEmailToken = await _userAccountRepository.GenerateChangeEmailTokenAsync(currentUser!, changeEmailDTO.Email);

               var recipientName = currentUser!.FullName!;
               var recipientEmail = changeEmailDTO.Email;

               var param = new Dictionary<string, string?>
               {
                    {"token", changeEmailToken },
                    {"email", changeEmailDTO.Email }
               };

               var changeEmailLink = Environment.GetEnvironmentVariable("EMAIL_CHANGE_EMAIL_URL") ?? throw new ApiException("Email change link not found", System.Net.HttpStatusCode.InternalServerError);

               var callback = QueryHelpers.AddQueryString(changeEmailLink!, param);

               var decodedCallback = Uri.UnescapeDataString(callback);

               var message = EmailTemplates.GetChangeEmail(recipientName, decodedCallback);

               _ = Task.Run(async () =>
               {

                    (var success, var errorMsg) = await _emailSender.SendEmailAsync(recipientName, recipientEmail,
                        "Bonheur Change Email", message);

                    if (!success) _logger.LogError($"Failed to send email with {recipientEmail}: {errorMsg}");

               });

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Change email link sent successfully",
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

        public async Task<ApplicationResponse> ChangeEmailAsync(string newEmail, string token)
        {
            try
            {
                var currentUser = await GetCurrentUser();

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newEmail)) throw new ApiException("Invalid token or email", System.Net.HttpStatusCode.BadRequest);

                var result = await _userAccountRepository.ChangeEmailAsync(currentUser!, newEmail, token);

                if (!result.Succeeded) throw new ApiException(string.Join("; ", result.Errors.Select(error => error.Description)), System.Net.HttpStatusCode.BadRequest);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Email changed successfully",
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
