using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.SocialNetwork;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/social-networks")]
    [Authorize]
    public class SocialNetworkController : ControllerBase
    {
        private readonly ISocialNetworkService _socialNetworkService;

        public SocialNetworkController(ISocialNetworkService socialNetworkService)
        {
            _socialNetworkService = socialNetworkService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        public async Task<IActionResult> GetSocialNetworks()
        {
            return Ok(await _socialNetworkService.GetSocialNetworks());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        public async Task<IActionResult> GetSocialNetworkById(int id)
        {
            return Ok(await _socialNetworkService.GetSocialNetworkById(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        public async Task<IActionResult> CreateSocialNetwor([FromForm] IFormFile file, [FromForm] string name)
        {
            return Ok(await _socialNetworkService.CreateSocialNetwork(file, name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        public async Task<IActionResult> UpdateSocialNetwork([FromForm] IFormFile? file, [FromForm] string name, int id)
        {
            return Ok(await _socialNetworkService.UpdateSocialNetwork(file, name, id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        public async Task<IActionResult> DeleteSocialNetwork(int id)
        {
            return Ok(await _socialNetworkService.DeleteSocialNetwork(id));
        }
    }
}
