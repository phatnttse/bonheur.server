using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface IBlogTagRepository
    {
        Task<IEnumerable<BlogTag>> GetBlogTagsAsync();
        Task<BlogTag> GetBlogTagByIdAsync(int id);
        Task<BlogTag> CreateBlogTagAsync(BlogTag blogTag);
        Task<BlogTag> UpdateBlogTagAsync(BlogTag blogTag);
        Task<BlogTag> DeleteBlogTagAsync(int id);
    }
}
