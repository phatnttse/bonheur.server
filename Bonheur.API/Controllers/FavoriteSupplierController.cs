using Bonheur.Services.DTOs.FavoriteSupplier;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/favorite-supplier")]
    public class FavoriteSupplierController : ControllerBase
    {
        private readonly IFavoriteSupplierService _favoriteSupplierService;

        public FavoriteSupplierController(IFavoriteSupplierService favoriteSupplierService)
        {
            _favoriteSupplierService = favoriteSupplierService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        //[Authorize(Roles = Constants.Roles.USER)]
        public async Task<IActionResult> GetFavoriteSupplierPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _favoriteSupplierService.GetAllFavoriteSuppliers(pageNumber, pageSize));
        }

        [HttpGet("category/{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        //[Authorize(Roles = Constants.Roles.USER)]
        public async Task<IActionResult> GetFavoriteSupplierPaginated([FromRoute] int categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _favoriteSupplierService.GetFavoriteSuppliersByCategoryId(categoryId, pageNumber, pageSize));
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddFavoriteSupplier([FromBody] int supplierId)
        {
            return Ok(await _favoriteSupplierService.AddFavoriteSupplier(supplierId));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.USER)]
        public async Task<IActionResult> GetFavoriteSupplier([FromRoute] int id)
        {
            return Ok(await _favoriteSupplierService.GetFavoriteSupplierAsync(id));
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = Constants.Roles.USER)]
        public async Task<IActionResult> DeleteFavoriteSupplier([FromRoute] int id)
        {
            return Ok(await _favoriteSupplierService.DeleteSupplierAsync(id));
        }
    }
}
