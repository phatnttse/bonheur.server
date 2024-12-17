using Bonheur.Services.DTOs.SupplierCategory;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/suppliers/categories")]
    [ApiVersion("1.0")]
    [Authorize]
    public class SupplierCategoryController : Controller
    {
        private readonly ISupplierCategoryService _supplierCategoryService;
        public SupplierCategoryController(ISupplierCategoryService supplierCategoryService)
        {
            _supplierCategoryService = supplierCategoryService;

        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllSupplierCategories()
        {
            return Ok(await _supplierCategoryService.GetAllSupplierCategoryAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSupplierCategoryById(int id)
        {
            return Ok(await _supplierCategoryService.GetSupplierCategoryByIdAsync(id));
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateNewSupplierCategory([FromBody] CreateSupplierCategoryDTO createSupplierCategoryDTO)
        {

            return Ok(await _supplierCategoryService.AddSupplierCategory(createSupplierCategoryDTO));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSupplierCategory([FromBody] SupplierCategoryDTO updateSupplierCategoryDTO, int id)
        {
            return Ok(await _supplierCategoryService.UpdateSupplierCategory(updateSupplierCategoryDTO, id));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSupplierCategory(int id)
        {
            return Ok(await _supplierCategoryService.DeleteSupplierCategory(id));
        }
    }
}
