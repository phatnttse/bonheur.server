using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.BlogPost;
using Bonheur.Services.DTOs.BlogTag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IBlogPostService
    {
        Task<ApplicationResponse> GetBlogPostsAsync(string? searchTitle, string? searchContent, int pageNumber = 1, int pageSize = 10);
        Task<ApplicationResponse> GetBlogPostByIdAsync(int id);
        Task<ApplicationResponse> GetBlogPostsByTags(List<BlogTagDTO> tags, int pageNumber = 1, int pageSize = 10);
        Task<ApplicationResponse> AddBlogPostAsync(BlogPostDTO blogPost);
        Task<ApplicationResponse> UpdateBlogPostAsync(int id, BlogPostDTO blogPost);
        Task<ApplicationResponse> DeleteBlogPostAsync(int id);
    }
}
