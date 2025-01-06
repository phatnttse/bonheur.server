using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.SubscriptionPackage;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/subscription-packages")]
    public class SubscriptionPackageController : ControllerBase
    {
        private readonly ISubscriptionPackageService _subscriptionPackageService;

        public SubscriptionPackageController(ISubscriptionPackageService subscriptionPackageService)
        {
            _subscriptionPackageService = subscriptionPackageService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSubscriptionPackageByIdAsync(int id)
        {
            return Ok(await _subscriptionPackageService.GetSubscriptionPackageByIdAsync(id));
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        public async Task<IActionResult> GetAllSubscriptionPackagesAsync()
        {
            return Ok(await _subscriptionPackageService.GetAllSubscriptionPackagesAsync());
        }

        [HttpPost]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateSubscriptionPackageAsync([FromBody] SubscriptionPackageDTO subscriptionPackageDTO)
        {
            return Ok(await _subscriptionPackageService.CreateSubscriptionPackageAsync(subscriptionPackageDTO));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSubscriptionPackageAsync(int id, [FromBody] SubscriptionPackageDTO subscriptionPackageDTO)
        {
            return Ok(await _subscriptionPackageService.UpdateSubscriptionPackageAsync(id, subscriptionPackageDTO));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSubscriptionPackageAsync(int id)
        {
            return Ok(await _subscriptionPackageService.DeleteSubscriptionPackageAsync(id));
        }
    }
}
