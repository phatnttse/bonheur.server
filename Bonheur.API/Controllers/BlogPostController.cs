using Bonheur.BusinessObjects.Models;
using Bonheur.Services;
using Bonheur.Services.DTOs.BlogPost;
using Bonheur.Services.DTOs.BlogTag;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/blog-post")]
    public class BlogPostController : ControllerBase
    {
       private readonly IBlogPostService _blogPostService;

        public BlogPostController(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetBlogPostsAsync([FromQuery] string? searchTitle, [FromQuery] string? searchContent, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _blogPostService.GetBlogPostsAsync(searchTitle, searchContent, pageNumber, pageSize));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetBlogPostByIdAsync([FromRoute] int id)
        {
            return Ok(await _blogPostService.GetBlogPostByIdAsync(id));
        }

        [HttpGet("tags")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetBlogPostByTagsAsync([FromQuery] List<BlogTagDTO> tags, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _blogPostService.GetBlogPostsByTags(tags, pageNumber, pageSize));
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddBlogPostAsync([FromBody] BlogPostDTO blogPostDTO)
        {
            return Ok(await _blogPostService.AddBlogPostAsync(blogPostDTO));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateBlogPostAsync([FromRoute] int id, [FromBody] BlogPostDTO blogPostDTO)
        {
            return Ok(await _blogPostService.UpdateBlogPostAsync(id, blogPostDTO));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteBlogPostAsync([FromRoute] int id)
        {
            return Ok(await _blogPostService.DeleteBlogPostAsync(id));
        }
    }
}
