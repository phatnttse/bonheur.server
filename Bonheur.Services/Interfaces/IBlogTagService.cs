using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.BlogTag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IBlogTagService
    {
        Task<ApplicationResponse> GetBlogTagsAsync();
        Task<ApplicationResponse> GetBlogTagByIdAsync(int id);
        Task<ApplicationResponse> CreateBlogTagAsync(BlogTagDTO blogTagDTO);
        Task<ApplicationResponse> UpdateBlogTagAsync(int id, BlogTagDTO blogTagDTO);
        Task<ApplicationResponse> DeleteBlogTagAsync(int id);
    }
}
