using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Account;
using System.Security.Claims;


namespace Bonheur.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Microsoft.AspNetCore.Identity.SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure);
        Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(ApplicationUser user, IEnumerable<string> scopes);
        Task<bool> CanSignInAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<ApplicationUser> GetUserByUsername(string username);
        Task<ApplicationResponse> SignUpUserAccount(CreateAccountDTO createAccountDTO);
    }
}
