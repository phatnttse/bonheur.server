using Bonheur.Services.DTOs.RequestPricing;
using Bonheur.Services.Interfaces;
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


    }
}
