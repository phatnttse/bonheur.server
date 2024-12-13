using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.PaginationParams;
using Bonheur.Services.DTOs.RequestPricing;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/request-pricing")]
    public class RequestPricingController : Controller
    {
        private readonly IRequestPricingsService _requestPricingsService;
        public RequestPricingController(IRequestPricingsService requestPricingsService)
        {
                _requestPricingsService = requestPricingsService;
        }

        

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateRequestPricing([FromBody] CreateRequestPricingDTO createRequestPricingDTO)
        {
            return Ok(await _requestPricingsService.CreateRequestPricing(createRequestPricingDTO));
        }

        [HttpGet("admin")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllRequestPricings([FromQuery] int pageNumber =1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _requestPricingsService.GetAllRequestPricing(pageNumber, pageSize));
        }

        [HttpGet("supplier")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = "SUPPLIER")]
        public async Task<IActionResult> GetAllRequestPricingsBySupplierId([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _requestPricingsService.GetAllRequestPricingBySupplierId(pageNumber, pageSize));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize]
        public async Task<IActionResult> GetRequestPricingById(int id)
        {
            return Ok(await _requestPricingsService.GetRequestPricingById(id));
        }

        [HttpPut("status/{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = "SUPPLIER")]
        public async Task<IActionResult> UpdateRequestPricingStatus(int id, [FromBody] UpdateRequestPricingStatusDTO status)
        {
            return Ok(await _requestPricingsService.UpdateRequestPricingStatus(id, status.Status));
        }
    }
}
