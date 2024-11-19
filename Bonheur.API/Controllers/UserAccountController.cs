using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
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
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(string id)
        {
            return Ok(await _userAccountService.GetUserByIdAsync(id));
        }


    }
}
