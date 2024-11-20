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
            var response = await _supplierCategoryService.GetAllSupplierCategoryAsync();            
            return Ok(response);
        }

        [HttpGet("GetSupplierCategory/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSupplierCategoryById(int id)
        {
            var response = await _supplierCategoryService.GetSupplierCategoryByIdAsync(id);

            if (response == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = $"Supplier category with id {id} not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Supplier category retrieved successfully",
                Data = response
            });
        }

        [HttpPost("CreateNewSupplierCategory")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateNewSupplierCategory([FromBody] CreateSupplierCategoryDTO createSupplierCategoryDTO)
        {
            var response = await _supplierCategoryService.AddSupplierCategory(createSupplierCategoryDTO);

            return Ok(response);
        }

        [HttpPut("UpdateSupplierCategory/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateSupplierCategory([FromBody] CreateSupplierCategoryDTO updateSupplierCategoryDTO, int id)
        {
            var response = await _supplierCategoryService.UpdateSupplierCategory(updateSupplierCategoryDTO, id);
            return Ok(response);
        }

        [HttpDelete("DeleteSupplierCategory/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSupplierCategory(int id)
        {
            var response = await _supplierCategoryService.DeleteSupplierCategory(id);
            return Ok(response);
        }
    }
}
