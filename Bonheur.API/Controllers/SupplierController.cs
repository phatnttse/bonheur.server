using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierName"></param>
        /// <param name="supplierCategoryId"></param>
        /// <param name="province"></param>
        /// <param name="isFeatured"></param>
        /// <param name="averageRating"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="sortAsc"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
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
            [FromQuery] string? orderBy,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {         
            return Ok(await _supplierService.GetSuppliersAsync(supplierName,
                supplierCategoryId,
                province,
                isFeatured,
                averageRating,
                minPrice,
                maxPrice,
                sortAsc,
                orderBy,
                pageNumber,
                pageSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            return Ok(await _supplierService.GetSupplierByIdAsync(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("users/{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> GetSupplierByUserId(string id)
        {
            return Ok(await _supplierService.GetSupplierByUserIdAsync(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createSupplierDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.USER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> SignupToBecomeSupplier([FromBody]CreateSupplierDTO createSupplierDTO)
        {
            return Ok(await _supplierService.CreateSupplierAsync(createSupplierDTO));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateSupplierProfileDTO"></param>
        /// <returns></returns>
        [HttpPatch("update/profile")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> UpdateSupplierProfile([FromBody] UpdateSupplierProfileDTO updateSupplierProfileDTO)
        {
            return Ok(await _supplierService.UpdateSupplierProfileAsync(updateSupplierProfileDTO));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateSupplierAddressDTO"></param>
        /// <returns></returns>
        [HttpPatch("update/address")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> UpdateSupplierAddress([FromBody] UpdateSupplierAddressDTO updateSupplierAddressDTO)
        {
            return Ok(await _supplierService.UpdateSupplierAddressAsync(updateSupplierAddressDTO));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="primaryImageIndex"></param>
        /// <returns></returns>
        [HttpPost("images/upload")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> UploadSupplierImages([FromForm] List<IFormFile> files, [FromForm] int? primaryImageIndex)
        {
            return Ok(await _supplierService.UploadSupplierImages(files, primaryImageIndex));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        [HttpGet("images/{supplierId}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> GetSupplierImagesBySupplier(int supplierId)
        {
            return Ok(await _supplierService.GetSupplierImagesBySupplier(supplierId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierImageId"></param>
        /// <returns></returns>
        [HttpDelete("images/{supplierImageId}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> DeleteSupplierImage(int supplierImageId)
        {
            return Ok(await _supplierService.DeleteSupplierImageAsync(supplierImageId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [HttpPatch("images/update/primary/{imageId}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> UpdatePrimaryImage(int imageId)
        {
            return Ok(await _supplierService.UpdatePrimaryImageAsync(imageId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPatch("status/{supplierId}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> UpdateSupplierStatus(int supplierId, [FromQuery] SupplierStatus status)
        {
            return Ok(await _supplierService.UpdateSupplierStatus(supplierId, status));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [HttpGet("slug/{slug}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSupplierBySlug(string slug)
        {
            return Ok(await _supplierService.GetSupplierBySlugAsync(slug));
        }

        /// <summary>
        /// Export supplier list to excel
        /// </summary>
        /// <returns></returns>
        [HttpGet("export/excel")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> ExportSupplierListToExcel()
        {
            var fileBytes = await _supplierService.ExportSupplierListToExcel();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SupplierList.xlsx");
        }

        /// <summary>
        /// Get suppliers API for admin
        /// </summary>
        /// <param name="supplierName"></param>
        /// <param name="supplierCategoryId"></param>
        /// <param name="province"></param>
        /// <param name="isFeatured"></param>
        /// <param name="averageRating"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="status"></param>
        /// <param name="sortAsc"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("admin")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> GetSuppliersByAdmin(
            [FromQuery] string? supplierName,
            [FromQuery] int? supplierCategoryId,
            [FromQuery] string? province,
            [FromQuery] bool? isFeatured,
            [FromQuery] decimal? averageRating,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] SupplierStatus? status,
            [FromQuery] bool? sortAsc,
            [FromQuery] string? orderBy,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            return Ok(await _supplierService.GetSuppliersByAdminAsync(supplierName,
                supplierCategoryId,
                province,
                isFeatured,
                averageRating,
                minPrice,
                maxPrice,
                status,
                sortAsc,
                orderBy,
                pageNumber,
                pageSize));
        }
    }
}
