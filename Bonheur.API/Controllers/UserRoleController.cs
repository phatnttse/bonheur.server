using Bonheur.Services.DTOs.UserRole;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using static Bonheur.Utils.Constants;

namespace Bonheur.API.Controllers
{

    [ApiController]
    [Route("api/v{version:apiVersion}/account")]
    [ApiVersion("1.0")]
    [Authorize]
    public class UserRoleController : Controller
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createUserRoleDTO"></param>
        /// <returns></returns>
        [HttpPost("roles")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateRole([FromBody] CreateUserRoleDTO createUserRoleDTO)
        {
            return Ok(await _userRoleService.CreateRoleAsync(createUserRoleDTO, createUserRoleDTO.Permissions?.Select(p => p.Value!).ToArray() ?? []));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("roles")]
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
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetRoles(int pageNumber, int pageSize)
        {
            return Ok(await _userRoleService.GetRolesLoadRelatedAsync(pageNumber, pageSize));
        }
    }
}
