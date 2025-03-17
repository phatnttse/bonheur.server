using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.BlogTag;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/blog-tag")]
    public class BlogTagController : ControllerBase
    {
        private readonly IBlogTagService _blogTagService;

        public BlogTagController(IBlogTagService blogTagService)
        {
            _blogTagService = blogTagService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetBlogTagsAsync()
        {
            return  Ok(await _blogTagService.GetBlogTagsAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetBlogTagByIdAsync([FromRoute] int id)
        {
            return Ok(await _blogTagService.GetBlogTagByIdAsync(id));
        }


        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddBlogTagAsync([FromBody] BlogTagDTO blogTagDTO)
        {
            return Ok(await _blogTagService.CreateBlogTagAsync(blogTagDTO));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateBlogTagAsync([FromRoute] int id, [FromBody] BlogTagDTO blogTagDTO)
        {
            return Ok(await _blogTagService.UpdateBlogTagAsync(id, blogTagDTO));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteBlogTagAsync([FromRoute] int id)
        {
            return Ok(await _blogTagService.DeleteBlogTagAsync(id));
        }
    }
}
