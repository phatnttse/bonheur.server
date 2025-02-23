using Bonheur.Services.DTOs.RequestPricing;
using Bonheur.Services;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bonheur.Services.DTOs.Review;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Bonheur.Utils;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/review")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        //[Authorize(Roles = Constants.Roles.USER)]
        public async Task<IActionResult> CreateReview([FromBody] CreateUpdateReviewDTO reviewDTO)
        {
            return Ok(await _reviewService.AddNewReview(reviewDTO));

        }

        [HttpPost("request-review")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize]
        public async Task<IActionResult> RequestToReview([FromBody] SendEmailReviewDTO sendEmailReviewDTO)
        {
            return Ok(await _reviewService.SendEmailRequestReview(sendEmailReviewDTO));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.USER)]
        public async Task<IActionResult> UpdateReview([FromBody] CreateUpdateReviewDTO reviewDTO, int id)
        {
            return Ok(await _reviewService.UpdateReview(reviewDTO, id));
        }


        [HttpGet("supplier/{supplierId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetReviews([FromRoute] int supplierId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            return Ok(await _reviewService.GetReviews(supplierId, pageNumber, pageSize));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetReviewById(int id)
        {
            return Ok(await _reviewService.GetReviewById(id));
        }

        [HttpGet("average-rating/{supplierId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetReviewsAverage([FromRoute] int supplierId)
        {
            return Ok(await _reviewService.GetReviewsAverage(supplierId));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> DeleteReview(int id)
        {
            return Ok(await _reviewService.DeleteReview(id));
        }

    }
}
