using Bonheur.BusinessObjects.Entities;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Utils;
using AutoMapper;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.Email;
using Microsoft.AspNetCore.WebUtilities;

namespace Bonheur.Services
{
    public class AuthService : IAuthService
    {
        private SignInManager<ApplicationUser> _signInManager;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;


        public AuthService(SignInManager<ApplicationUser> signInManager, IUserAccountRepository userAccountRepository, IMapper mapper, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userAccountRepository = userAccountRepository;
            _mapper = mapper;
            _emailSender = emailSender;

        }
        public Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure)
        {
            try
            {
                return _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(ApplicationUser user, IEnumerable<string> scopes)
        {
            try
            {
                var principal = await _signInManager.CreateUserPrincipalAsync(user);
                principal.SetScopes(scopes);

                var identity = principal.Identity as ClaimsIdentity
                    ?? throw new InvalidOperationException("The ClaimsPrincipal's Identity is null.");

                if (user.FullName != null) identity.SetClaim(CustomClaims.FullName, user.FullName);

                principal.SetDestinations(GetDestinations);

                return principal;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public  IEnumerable<string> GetDestinations(Claim claim)
        {
            if (claim.Subject == null)
                throw new InvalidOperationException("The Claim's Subject is null.");

            switch (claim.Type)
            {
                case Claims.Name:
                    if (claim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Email:
                    if (claim.Subject.HasScope(Scopes.Email))
                        yield return Destinations.IdentityToken;

                    yield break;

                case CustomClaims.FullName:
                    if (claim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case CustomClaims.Configuration:
                    if (claim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (claim.Subject.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;

                    yield break;

                case CustomClaims.Permission:
                    yield return Destinations.AccessToken;

                    if (claim.Subject.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;

                    yield break;

                // IdentityOptions.ClaimsIdentity.SecurityStampClaimType
                case "AspNet.Identity.SecurityStamp":
                    // Never include the security stamp in the access and identity tokens, as it's a secret value.
                    yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }

        public Task<bool> CanSignInAsync(ApplicationUser user)
        {
            try
            {
                return _signInManager.CanSignInAsync(user);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            try
            {
                return await _userAccountRepository.GetUserByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApplicationUser> GetUserByUsername(string username)
        {
            try
            {
                return await _userAccountRepository.GetUserByUserNameAsync(username);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApplicationResponse> SignUpUserAccount(CreateAccountDTO createAccountDTO)
        {
            try
            {
                var existingUser = await _userAccountRepository.GetUserByUserNameAsync(createAccountDTO.Email!);

                if (existingUser != null)
                {
                    if (!existingUser.EmailConfirmed) throw new ApiException("Email is registered and not authenticated. Check your mailbox!", System.Net.HttpStatusCode.BadRequest);

                    throw new ApiException("Email has been registered", System.Net.HttpStatusCode.BadRequest);
                }

                var user = _mapper.Map<ApplicationUser>(createAccountDTO);

                var result = await _userAccountRepository.CreateUserAsync(user, new string[] { Constants.Roles.USER }, createAccountDTO.Password!);

                if (!result.Succeeded)
                {
                    throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);
                }

                var recipientName = user.FullName!;
                var recipientEmail = user.Email!;

                var token = await _userAccountRepository.GenereEmailConfirmationTokenAsync(user);

                var param = new Dictionary<string, string?>
                {
                    {"token", token },
                    {"email", user.Email }
                };

                var confirmationLink = Environment.GetEnvironmentVariable("EMAIL_CONFIRMATION_URL");

                var callback = QueryHelpers.AddQueryString(confirmationLink!, param);

                var message = EmailTemplates.GetConfirmEmail(recipientName, callback);

                (var success, var errorMsg) = await _emailSender.SendEmailAsync(recipientName, recipientEmail,
                    "Bonheur Confirm Email", message);

                if (!success) throw new ApiException(errorMsg ?? "Send email failed", System.Net.HttpStatusCode.InternalServerError);

                return new ApplicationResponse
                {
                    Message = "Please check your mailbox to confirm your email and complete registration.",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public async Task<ApplicationResponse> ConfirmEmail(string email, string token)
        {
            try
            {
                var existingUser = await _userAccountRepository.GetUserByUserNameAsync(email);

                if (existingUser == null) throw new ApiException("Invalid email confirmation request", System.Net.HttpStatusCode.BadRequest);

                var result = await _userAccountRepository.ConfirmEmailAsync(existingUser, token);

                if (!result.Succeeded)
                {
                    throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);
                }

                return new ApplicationResponse
                {
                    Message = "Email confirmed successfully. Sign in now!",
                    Success = true,
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


        public async Task<ApplicationResponse> ResetPasswordAsync(string email, string resetToken, string newPassword)
        {
            try
            {
                var existingUser = await _userAccountRepository.GetUserByUserNameAsync(email);

                if (existingUser == null) throw new ApiException("Invalid password reset request", System.Net.HttpStatusCode.BadRequest);

                var result = await _userAccountRepository.ResetPasswordAsync(existingUser!, resetToken, newPassword);

                if (!result.Succeeded) throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);

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

        public async Task<ApplicationResponse> ForgotPasswordAsync(string email)
        {
            try
            {
                var existingUser = await _userAccountRepository.GetUserByUserNameAsync(email);

                if (existingUser == null) throw new ApiException("Invalid password reset request", System.Net.HttpStatusCode.BadRequest);

                var resetToken = await _userAccountRepository.GeneratePasswordResetTokenAsync(existingUser);

                var recipientName = existingUser.FullName!;
                var recipientEmail = existingUser.Email!;

                var param = new Dictionary<string, string?>
                {
                    {"token", resetToken },
                    {"email", existingUser.Email }
                };

                var resetPasswordLink = Environment.GetEnvironmentVariable("EMAIL_RESET_PASSWORD_URL");

                var callback = QueryHelpers.AddQueryString(resetPasswordLink!, param);

                var decodedCallback = Uri.UnescapeDataString(callback);


                var message = EmailTemplates.GetResetPasswordEmail(recipientName, decodedCallback);

                (var success, var errorMsg) = await _emailSender.SendEmailAsync(recipientName, recipientEmail,
                    "Bonheur Reset Password", message);

                if (!success) throw new ApiException(errorMsg ?? "Send email failed", System.Net.HttpStatusCode.InternalServerError);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Reset password email is being sent to you. Please check your mailbox",
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


        public async Task<ApplicationUser> HandleGoogleLoginAsync(GoogleAccountDTO googleAccountDTO)
        {
            try
            {
                var existingUser = await _userAccountRepository.GetUserByUserNameAsync(googleAccountDTO.Email!);

                if (existingUser != null)
                {
                    // Kiểm tra liên kết tài khoản Google
                    var existingLogin = await _userAccountRepository.GetUserLoginAsync(Constants.Providers.GOOGLE, googleAccountDTO.GoogleId!);
                    if (existingLogin == null)
                    {
                        // Thêm liên kết nếu chưa tồn tại
                        await _userAccountRepository.AddLoginAsync(existingUser, new UserLoginInfo(Constants.Providers.GOOGLE, googleAccountDTO.GoogleId!, Constants.Providers.GOOGLE));
                    }
                    return existingUser;
                }

                var user = _mapper.Map<ApplicationUser>(googleAccountDTO);

                var result = await _userAccountRepository.CreateUserNotPassword(user, new string[] { Constants.Roles.USER });

                if (!result.Succeeded)
                {
                    throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);
                }

                // Liên kết tài khoản Google với tài khoản mới
                await _userAccountRepository.AddLoginAsync(user, new UserLoginInfo(Constants.Providers.GOOGLE, googleAccountDTO.GoogleId!, Constants.Providers.GOOGLE));

                return user;

            }
            catch (ApiException ex)
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
