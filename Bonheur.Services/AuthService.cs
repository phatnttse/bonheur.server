using Bonheur.BusinessObjects.Entities;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Utils;
using Bonheur.Services.DTOs.UserAccount;
using AutoMapper;

namespace Bonheur.Services
{
    public class AuthService : IAuthService
    {
        private SignInManager<ApplicationUser> _signInManager;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IMapper _mapper;


        public AuthService(SignInManager<ApplicationUser> signInManager, IUserAccountRepository userAccountRepository, IMapper mapper)
        {
            _signInManager = signInManager;
            _userAccountRepository = userAccountRepository;
            _mapper = mapper;

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

        public async Task<ApplicationResponse> SignUp(CreateAccountDTO createAccountDTO)
        {
            try
            {

                var user = _mapper.Map<ApplicationUser>(createAccountDTO);

                var result = await _userAccountRepository.CreateUserAsync(user, new string[] { Constants.Roles.USER }, createAccountDTO.Password!);

                if (!result.Succeeded)
                {
                    throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);
                }

                return new ApplicationResponse
                {
                    Message = "User created successfully",
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
    } 
}
