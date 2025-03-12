using Bonheur.Services.DTOs.Storage;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/storage")]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadAsync([FromForm] IFormFile upload)
        {
            AzureBlobResponseDTO response = await _storageService.UploadAsync(upload);
            return Ok(new { url = response.Blob.Uri });
        }
    }
}
