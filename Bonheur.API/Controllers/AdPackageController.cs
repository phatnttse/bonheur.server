using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.AdPackage;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ad-package")]
    public class AdPackageController : ControllerBase
    {
        private readonly IAdPackageService _adPackageService;
        public AdPackageController(IAdPackageService adPackageService)
        {
            _adPackageService = adPackageService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllAdPackagesAsync([FromQuery] string? adPackageTitle, [FromQuery] int pageNumber=1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _adPackageService.GetAdPackagesAsync(adPackageTitle, pageNumber, pageSize));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> GetAdPackageById([FromRoute] int id)
        {
            return Ok(await _adPackageService.GetAdPackageById(id));
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> CreateAdPackage([FromBody] AdPackageDTO adPackageDTO)
        {
            return Ok(await _adPackageService.AddAdPackage(adPackageDTO));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> UpdateAdPackage([FromRoute] int id,[FromBody] AdPackageDTO adPackageDTO)
        {
            return Ok(await _adPackageService.UpdateAdPackage(id, adPackageDTO));   
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> DeleteAdPackage([FromRoute] int id)
        {
            return Ok(await _adPackageService.DeleteAdPackage(id)); 
        }
    }
}
