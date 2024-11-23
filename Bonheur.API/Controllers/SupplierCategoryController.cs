using Bonheur.Services.DTOs.SupplierCategory;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/supplierCategory")]
    [ApiVersion("1.0")]
    [Authorize]
    public class SupplierCategoryController : Controller
    {
        private readonly ISupplierCategoryService _supplierCategoryService;
        public SupplierCategoryController(ISupplierCategoryService supplierCategoryService)
        {
            _supplierCategoryService = supplierCategoryService;

        }

        [HttpGet("GetAllSupplierCategory")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllSupplierCategories()
        {
            return Ok(await _supplierCategoryService.GetAllSupplierCategoryAsync());
        }

        [HttpGet("GetSupplierCategory/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSupplierCategoryById(int id)
        {
            return Ok(await _supplierCategoryService.GetSupplierCategoryByIdAsync(id));
        }

        [HttpPost("CreateNewSupplierCategory")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateNewSupplierCategory([FromBody] CreateSupplierCategoryDTO createSupplierCategoryDTO)
        {

            return Ok(await _supplierCategoryService.AddSupplierCategory(createSupplierCategoryDTO));
        }

        [HttpPut("UpdateSupplierCategory/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateSupplierCategory([FromBody] CreateSupplierCategoryDTO updateSupplierCategoryDTO, int id)
        {
            return Ok(await _supplierCategoryService.UpdateSupplierCategory(updateSupplierCategoryDTO, id));
        }

        [HttpDelete("DeleteSupplierCategory/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSupplierCategory(int id)
        {
            return Ok(await _supplierCategoryService.DeleteSupplierCategory(id));
        }
    }
}
