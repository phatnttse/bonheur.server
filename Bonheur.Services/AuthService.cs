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
using Google.Apis.Auth;
using Bonheur.BusinessObjects.Enums;
using Microsoft.Extensions.Logging;

namespace Bonheur.Services
{
    public class AuthService : IAuthService
    {
        private SignInManager<ApplicationUser> _signInManager;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AuthService> _logger;


        public AuthService(SignInManager<ApplicationUser> signInManager, IUserAccountRepository userAccountRepository, IMapper mapper, IEmailSender emailSender, ILogger<AuthService> logger)
        {
            _signInManager = signInManager;
            _userAccountRepository = userAccountRepository;
            _mapper = mapper;
            _emailSender = emailSender;
            _logger = logger;

        }
        public async Task<ApplicationUser> HandleLoginAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    throw new ApiException("Please provide a valid email and password", System.Net.HttpStatusCode.BadRequest);

                var user = _userAccountRepository.GetUserByUserNameAsync(username).Result;

                if (user == null)
                    throw new ApiException("Please check that your email and password is correct.", System.Net.HttpStatusCode.BadRequest);

                if (!user.EmailConfirmed) throw new ApiException("Email has not been confirmed. Please check your mailbox", System.Net.HttpStatusCode.BadRequest);

                if (!user.IsEnabled) throw new ApiException("The specified user account is disabled.", System.Net.HttpStatusCode.BadRequest);

                SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, true);

                if (result.IsLockedOut) throw new ApiException("The specified user account has been locked.", System.Net.HttpStatusCode.BadRequest);

                if (result.IsNotAllowed) throw new ApiException("The specified user account is not allowed to sign in.", System.Net.HttpStatusCode.BadRequest);

                if (!result.Succeeded) throw new ApiException("Please check that your email and password is correct.", System.Net.HttpStatusCode.BadRequest);

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

        public async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(ApplicationUser user, IEnumerable<string> scopes)
        {
            try
            {
                var principal = await _signInManager.CreateUserPrincipalAsync(user);
                principal.SetScopes(scopes);

                var identity = principal.Identity as ClaimsIdentity
                    ?? throw new InvalidOperationException("The ClaimsPrincipal's Identity is null.");

                if (user.FullName != null) identity.SetClaim(CustomClaims.FullName, user.FullName);

                if (user.Gender != null) identity.SetClaim(CustomClaims.Gender, user.Gender.ToString());

                if (user.EmailConfirmed) identity.SetClaim(CustomClaims.EmailConfirmed, user.EmailConfirmed.ToString());

                if (user.PictureUrl != null) identity.SetClaim(CustomClaims.PictureUrl, user.PictureUrl);

                principal.SetDestinations(GetDestinations);

                return principal;

            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
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

                case CustomClaims.Gender:
                    if (claim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case CustomClaims.EmailConfirmed:
                    if (claim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case CustomClaims.PictureUrl:
                    if (claim.Subject.HasScope(Scopes.Profile))
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

        public async Task<bool> CanSignInAsync(ApplicationUser user)
        {
            try
            {
               
                if (!await _signInManager.CanSignInAsync(user))
                {
                    throw new ApiException("The specified user account is not allowed to sign in.", System.Net.HttpStatusCode.BadRequest);
                }

                return true;
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


        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _userAccountRepository.GetUserByIdAsync(id);
                
                if (user == null) throw new ApiException("The refresh token is no longer valid.", System.Net.HttpStatusCode.NotFound);

                if (user.IsLockedOut) throw new ApiException("The specified user account has been locked.", System.Net.HttpStatusCode.BadRequest);

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

                switch (createAccountDTO.Gender)
                {
                    case Gender.MALE:
                        user.PictureUrl = Constants.AVATAR_DEFAULT.MALE;
                        break;
                    case Gender.FEMALE:
                        user.PictureUrl = Constants.AVATAR_DEFAULT.FEMAILE;
                        break;
                    default:
                        user.PictureUrl = Constants.AVATAR_DEFAULT.OTHER;
                        break;
                }

                var result = await _userAccountRepository.CreateUserAsync(user, new string[] { Constants.Roles.USER }, createAccountDTO.Password!);

                if (!result.Succeeded)
                {
                    throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);
                }

                // Send email asynchronously in background
                _ = Task.Run(async () =>
                {
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

                    var (success, errorMsg) = await _emailSender.SendEmailAsync(recipientName, recipientEmail,
                        "Bonheur Confirm Email", message);
                    if (!success)
                    {
                        _logger.LogError($"Failed to send email with {recipientEmail}: {errorMsg}");
                    }
                });

                return new ApplicationResponse
                {
                    Message = "Please check your mailbox to confirm your email and complete registration.",
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

                _ = Task.Run(async () =>
                {

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

                    if (!success) _logger.LogError($"Failed to send email with {recipientEmail}: {errorMsg}");

                });

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


        public async Task<ApplicationUser> HandleSocialLoginAsync(string assertion, string provider)
        {
            try
            {
                if (provider == Constants.Providers.GOOGLE)
                {
                    var googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
                    if (string.IsNullOrEmpty(googleClientId))
                    {
                        throw new ApiException("Google Client ID is not configured.", System.Net.HttpStatusCode.InternalServerError);
                    }

                    var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { googleClientId }
                    };

                    var payload = await GoogleJsonWebSignature.ValidateAsync(assertion, validationSettings);

                    var googleAccountDTO = new GoogleAccountDTO
                    {
                        GoogleId = payload.Subject,
                        Email = payload.Email,
                        FullName = payload.Name,
                        PictureUrl = payload.Picture,
                        EmailConfirmed = payload.EmailVerified
                    };

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

                throw new ApiException("Invalid social provider", System.Net.HttpStatusCode.BadRequest);

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
