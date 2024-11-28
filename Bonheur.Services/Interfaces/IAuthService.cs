using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Account;
using System.Security.Claims;


namespace Bonheur.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApplicationUser> HandleLoginAsync(string username, string password);
        Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(ApplicationUser user, IEnumerable<string> scopes);
        Task<bool> CanSignInAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<ApplicationResponse> SignUpUserAccount(CreateAccountDTO createAccountDTO);
        Task<ApplicationResponse> ConfirmEmail(string email, string token);
        Task<ApplicationUser> HandleGoogleLoginAsync(GoogleAccountDTO googleAccountDTO);
        Task<ApplicationResponse> ResetPasswordAsync(string email, string resetToken, string newPassword);
        Task<ApplicationResponse> ForgotPasswordAsync(string email);
    }
}
