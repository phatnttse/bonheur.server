using Bonheur.Services.DTOs.Account;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;


namespace Bonheur.API.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsPasswordGrantType())
            {

                var user = await _authService.HandleLoginAsync(request.Username, request.Password);

                var principal = await _authService.CreateClaimsPrincipalAsync(user, request.GetScopes());

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            else if (request.IsRefreshTokenGrantType())
            {
                var result =  await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                var userId = result?.Principal?.GetClaim(Claims.Subject);
                var user = userId != null ? await _authService.GetUserByIdAsync(userId) : null;

                await _authService.CanSignInAsync(user!);
                
                var scopes = request.GetScopes();
                    if (scopes.Length == 0 && result?.Principal != null)
                        scopes = result.Principal.GetScopes();

                // Recreate the claims principal in case they changed since the refresh token was issued.
                var principal = await _authService.CreateClaimsPrincipalAsync(user!, scopes);
            
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new InvalidOperationException($"The specified grant type \"{request.GrantType}\" is not supported.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullname"></param>
        /// <param name="email"></param>
        /// <param name="gender"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("signup")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SignUpUserAccount([FromBody] CreateAccountDTO createAccountDTO)
        {
            return Ok(await _authService.SignUpUserAccount(createAccountDTO));
        }

        [HttpPost]
        [Route("confirm-email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailRequestDTO request)
        {
            return Ok(await _authService.ConfirmEmail(request.Email, request.Token));
        }

        [HttpPost]
        [Route("reset-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ResetPassword([FromBody] EmailRequestDTO request)
        {
            return Ok(await _authService.ResetPasswordAsync(request.Email, request.Token, request.Password!));
        }

        [HttpPost]
        [Route("forgot-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            return Ok(await _authService.ForgotPasswordAsync(email));
        }

        /// Unfinished
        [HttpGet("google")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// Unfinished
        [HttpGet("signin-google")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GoogleCallback()
        {
            // Lấy thông tin xác thực từ Google
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest("Google authentication failed.");

            // Lấy thông tin người dùng từ Google
            var googleAccountDTO = new GoogleAccountDTO
            {
                FullName = authenticateResult.Principal?.FindFirst(ClaimTypes.Name)?.Value,
                Email = authenticateResult.Principal?.FindFirst(ClaimTypes.Email)?.Value,
                GoogleId = authenticateResult.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                PictureUrl = authenticateResult.Principal?.FindFirst("urn:google:picture")?.Value
            };

            if (string.IsNullOrEmpty(googleAccountDTO.Email))
                return BadRequest("Google authentication did not return an email address.");

            // Kiểm tra hoặc tạo tài khoản người dùng trong hệ thống của bạn
            var user = await _authService.HandleGoogleLoginAsync(googleAccountDTO);

            // Lấy yêu cầu OpenIddict Server
            var request = HttpContext.GetOpenIddictServerRequest();

            var principal = await _authService.CreateClaimsPrincipalAsync(user, new string[] { });

            // Đăng nhập người dùng vào OpenIddict
            await HttpContext.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);

            return Redirect("https://your-app.com/dashboard");
        }

    }
}

