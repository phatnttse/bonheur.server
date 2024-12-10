using Bonheur.BusinessObjects.Models;
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

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Supplier")]
        public async Task<IActionResult> GetAllRequestPricings()
        {
            return Ok(await _requestPricingsService.GetAllRequestPricing());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Supplier")]
        public async Task<IActionResult> GetRequestPricingById(int id)
        {
            return Ok(await _requestPricingsService.GetRequestPricingById(id));
        }

        [HttpPut("status/{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Supplier")]
        public async Task<IActionResult> UpdateRequestPricingStatus(int id, [FromBody] UpdateRequestPricingStatusDTO status)
        {
            return Ok(await _requestPricingsService.UpdateRequestPricingStatus(id, status.Status));
        }
    }
}
