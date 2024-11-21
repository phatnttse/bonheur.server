using Bonheur.API.Authorization;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/account")]
    [ApiVersion("1.0")]
    [Authorize]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserAccountService _userAccountService;
        private readonly IAuthorizationService _authorizationService;

        public UserAccountController(IUserAccountService userAccountService, IAuthorizationService authorizationService)
        {
            _userAccountService = userAccountService;
            _authorizationService = authorizationService;
        }


        [HttpGet("users/me")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        public async Task<IActionResult> GetCurrentUser()
        {
            return Ok(await _userAccountService.GetCurrentUserAsync());
        }

        [HttpGet("users/{id}", Name = nameof(GetUserById))]
        [Authorize(AuthPolicies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(string id)
        {
            return Ok(await _userAccountService.GetUserAndRolesAsync(id));
        }


        [HttpGet("users/{pageNumber:int}/{pageSize:int}")]
        [Authorize(AuthPolicies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        public async Task<IActionResult> GetUsers(int pageNumber, int pageSize)
        {
            return Ok(await _userAccountService.GetUsersAndRolesAsync(pageNumber, pageSize));
        }


        [HttpPut("users/me")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserProfileDTO updateUserProfileDTO)
        {
            return Ok(await _userAccountService.UpdateCurrentUserAsync(updateUserProfileDTO));
        }


        [HttpPut("users/{id}")]
        [Authorize(AuthPolicies.ManageAllUsersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserAccountDTO userAccountDTO)
        {
            if (!(await _authorizationService.AuthorizeAsync(User, id,
               UserAccountManagementOperations.UpdateOperationRequirement)).Succeeded)
                return new ChallengeResult();

            return Ok(await _userAccountService.UpdateUserAndUserRoleAsync(id, userAccountDTO));
        }

        [HttpPatch("users/{id}/status")]
        [Authorize(AuthPolicies.ManageAllUsersPolicy)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUserAccountStatus(string id, [FromBody] UserAccountStatusDTO userAccountStatusDTO)
        {
            if (!(await _authorizationService.AuthorizeAsync(User, id,
               UserAccountManagementOperations.UpdateOperationRequirement)).Succeeded)
                return new ChallengeResult();

            return Ok(await _userAccountService.UpdateUserAccountStatusAsync(id, userAccountStatusDTO));
        }

        [HttpPatch("users/password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePassswordDTO updatePassswordDTO)
        {      
            return Ok(await _userAccountService.UpdatePasswordAsync(updatePassswordDTO.CurrentPassword, updatePassswordDTO.NewPassword));
        }

        [HttpPost("users/avatar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            return Ok(await _userAccountService.UploadAvatar(file));
        }

    }
}
