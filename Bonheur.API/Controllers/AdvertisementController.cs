using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Advertisement;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/advertisements")]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;

        public AdvertisementController(IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> GetAllAdvertisements([FromQuery] string? searchTitle, [FromQuery] string? searchContent, [FromQuery] AdvertisementStatus? status, [FromQuery] PaymentStatus? paymentStatus, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _advertisementService.GetAdvertisementsAsync(searchTitle, searchContent, status, paymentStatus, pageNumber, pageSize));
        }

        [HttpGet("supplier")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.SUPPLIER)]
        public async Task<IActionResult> GetAdvertisementsBySupplier([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _advertisementService.GetAdvertisementBySupplierAsync(pageNumber, pageSize));
        }

        [HttpGet("active")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetActiveAdvertisements([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _advertisementService.GetActiveAdvertisementsAsync(pageNumber, pageSize));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAdvertisementById([FromRoute] int id)
        {
            return Ok(await _advertisementService.GetAdvertisementByIdAsync(id));
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> CreateAdvertisement([FromForm] CreateAdvertisementDTO advertisementDTO)
        {
            return Ok(await _advertisementService.AddAdvertisementAsync(advertisementDTO));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> UpdateAdvertisement([FromRoute] int id, [FromForm] UpdateAdvertisementDTO advertisementDTO)
        {
            return Ok(await _advertisementService.UpdateAdvertisementAsync(id, advertisementDTO));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> DeleteAdvertisement([FromRoute] int id)
        {
            return Ok(await _advertisementService.DeleteAdvertisementAsync(id));
        }
    }
}
