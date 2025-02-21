using Bonheur.Services.DTOs.SupplierCategory;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/suppliers/categories")]
    [ApiVersion("1.0")]
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
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> GetSupplierCategoryById(int id)
        {
            return Ok(await _supplierCategoryService.GetSupplierCategoryByIdAsync(id));
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> CreateNewSupplierCategory([FromForm] IFormFile file, [FromForm] string name, [FromForm] string description)
        {

            return Ok(await _supplierCategoryService.AddSupplierCategory(file, name, description));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> UpdateSupplierCategory([FromForm] IFormFile? file, [FromForm] string name, [FromForm] string description, int id)
        {
            return Ok(await _supplierCategoryService.UpdateSupplierCategory(file, name, description, id));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> DeleteSupplierCategory(int id)
        {
            return Ok(await _supplierCategoryService.DeleteSupplierCategory(id));
        }
    }
}
