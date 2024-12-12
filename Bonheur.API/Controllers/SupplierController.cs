using Bonheur.BusinessObjects.Models;
using Bonheur.Services;
using Bonheur.Services.DTOs.Supplier;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/suppliers")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IRequestPricingsService _requestPricingsService;

        public SupplierController(ISupplierService supplierService, IRequestPricingsService requestPricingsService)
        {
            _supplierService = supplierService;
            _requestPricingsService = requestPricingsService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetSuppliers(
            [FromQuery] string? supplierName,
            [FromQuery] int? supplierCategoryId,
            [FromQuery] string? province,
            [FromQuery] bool? isFeatured,
            [FromQuery] decimal? averageRating,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? sortAsc,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _supplierService.GetSuppliersAsync(
                supplierName,
                supplierCategoryId,
                province,
                isFeatured,
                averageRating,
                minPrice,
                maxPrice,
                sortAsc,
                pageNumber,
                pageSize);

            return Ok(await _supplierService.GetSuppliersAsync(supplierName,
                supplierCategoryId,
                province,
                isFeatured,
                averageRating,
                minPrice,
                maxPrice,
                sortAsc,
                pageNumber,
                pageSize));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            return Ok(await _supplierService.GetSupplierByIdAsync(id));
        }

        [HttpGet("users/{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> GetSupplierByUserId(string id)
        {
            return Ok(await _supplierService.GetSupplierByUserIdAsync(id));
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.USER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> SignupToBecomeSupplier([FromBody]CreateSupplierDTO createSupplierDTO)
        {
            return Ok(await _supplierService.CreateSupplierAsync(createSupplierDTO));
        }

        [HttpPatch("update/profile")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> UpdateSupplierProfile([FromBody] UpdateSupplierProfileDTO updateSupplierProfileDTO)
        {
            return Ok(await _supplierService.UpdateSupplierProfileAsync(updateSupplierProfileDTO));
        }

        [HttpPatch("update/address")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> UpdateSupplierAddress([FromBody] UpdateSupplierAddressDTO updateSupplierAddressDTO)
        {
            return Ok(await _supplierService.UpdateSupplierAddressAsync(updateSupplierAddressDTO));
        }

        [HttpPost("images/upload")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> UploadSupplierImages([FromForm] List<IFormFile> files, [FromForm] int? primaryImageIndex)
        {
            return Ok(await _supplierService.UploadSupplierImages(files, primaryImageIndex));
        }

        [HttpGet("images/{supplierId}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> GetSupplierImagesBySupplier(int supplierId)
        {
            return Ok(await _supplierService.GetSupplierImagesBySupplier(supplierId));
        }

        [HttpDelete("images/{supplierImageId}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> DeleteSupplierImage(int supplierImageId)
        {
            return Ok(await _supplierService.DeleteSupplierImageAsync(supplierImageId));
        }

        [HttpPatch("images/update/primary/{imageId}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> UpdatePrimaryImage(int imageId)
        {
            return Ok(await _supplierService.UpdatePrimaryImageAsync(imageId));
        }

        [HttpGet("request-pricing")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize]
        public async Task<IActionResult> GetAllRequestPricings()
        {
            return Ok(await _requestPricingsService.GetAllRequestPricing());
        }

        [HttpGet("request-pricing/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize]
        public async Task<IActionResult> GetRequestPricingById(int id)
        {
            return Ok(await _requestPricingsService.GetRequestPricingById(id));
        }

        [HttpPut("request-pricing/update/status/responsed/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize]
        public async Task<IActionResult> RequestPricingResponsed(int id)
        {
            return Ok(await _requestPricingsService.RequestPricingResponsed(id));
        }

        [HttpPut("request-pricing/update/status/rejected/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize]
        public async Task<IActionResult> RequestPricingRejected(int id)
        {
            return Ok(await _requestPricingsService.RequestPricingRejected(id));
        }
    }
}
