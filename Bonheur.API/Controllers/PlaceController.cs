using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Bonheur.API.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/place")]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        /// <summary>
        /// Suggesting addresses based on Goong Map's Api
        /// </summary>
        /// <param name="input"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        [HttpGet("autocomplete")]
        [Authorize(Roles = Constants.Roles.SUPPLIER)]
        [EnableRateLimiting("global")]
        public async Task<IActionResult> GetAutocompletePlaces([FromQuery] string input, [FromQuery] string? location)
        {
            return Ok(await _placeService.AutoComplete(input, location));
        }
    }
}
