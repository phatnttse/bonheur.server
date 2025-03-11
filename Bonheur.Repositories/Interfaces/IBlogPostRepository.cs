using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<IPagedList<BlogPost>> GetBlogPostsAsync(string? searchTitle, string? searchContent, int pageNumber = 1, int pageSize = 10);
        Task<BlogPost?> GetBlogPostByIdAsync(int id);
        Task<IPagedList<BlogPost>> GetBlogPostsByTagsAsync(List<BlogTag> tags, int pageNumber = 1, int pageSize = 10);
        Task AddBlogPostAsync(BlogPost blogPost);
        Task UpdateBlogPostAsync(BlogPost blogPost);
        Task DeleteBlogPostAsync(BlogPost blogPost);
    }
}
