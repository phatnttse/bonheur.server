using Bonheur.API.Authorization;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;


namespace Bonheur.API.Controllers
{

    [ApiController]
    [Route("api/v{version:apiVersion}/account")]
    [ApiVersion("1.0")]
    //[Authorize]
    public class UserRoleController : Controller
    {
        private readonly IUserRoleService _userRoleService;
        private readonly IAuthorizationService _authorizationService;


        public UserRoleController(IUserRoleService userRoleService, IAuthorizationService authorizationService)
        {
            _userRoleService = userRoleService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createUserRoleDTO"></param>
        /// <returns></returns>
        [HttpPost("roles")]
        [Authorize(AuthPolicies.ManageAllRolesPolicy)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateRole([FromBody] UserRoleDTO createUserRoleDTO)
        {
            return Ok(await _userRoleService.CreateRoleAsync(createUserRoleDTO, createUserRoleDTO.Permissions?.Select(p => p.Value!).ToArray() ?? []));
        }

        [HttpGet("roles/{id}", Name = nameof(GetRoleById))]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleById(string id)
        {
            return Ok(await _userRoleService.GetRoleByIdAsync(id));
        }

        
        [HttpGet("roles")]
        //[Authorize(AuthPolicies.ViewAllRolesPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetRoles()
        {
            return await GetRoles(-1, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("roles/{pageNumber:int}/{pageSize:int}")]
        [Authorize(AuthPolicies.ViewAllRolesPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetRoles(int pageNumber, int pageSize)
        {
            return Ok(await _userRoleService.GetRolesLoadRelatedAsync(pageNumber, pageSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("roles/name/{name}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleByName(string name)
        {
            if (!(await _authorizationService.AuthorizeAsync(User, name,
                AuthPolicies.ViewRoleByRoleNamePolicy)).Succeeded)
                return new ChallengeResult();

            return Ok(await _userRoleService.GetRoleByNameAsync(name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPut("roles/{id}")]
        [Authorize(AuthPolicies.ManageAllRolesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UserRoleDTO role)
        {
            return Ok(await _userRoleService.UpdateRoleAsync(id, role, role.Permissions?.Select(p => p.Value!).ToArray() ?? []));
        }

        [HttpDelete("roles/{id}")]
        [Authorize(AuthPolicies.ManageAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            return Ok(await _userRoleService.DeleteRoleAsync(id));
        }

        [HttpGet("permissions")]
        [Authorize(AuthPolicies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        public IActionResult GetAllPermissions()
        {
            return Ok(_userRoleService.GetAllPermissions());
        }
    }
}
