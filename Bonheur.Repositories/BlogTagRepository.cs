using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories
{
    public class BlogTagRepository : IBlogTagRepository
    {
        private readonly BlogTagDAO _blogTagDAO;
        public BlogTagRepository(BlogTagDAO blogTagDAO)
        {
            _blogTagDAO = blogTagDAO;
        }
        public Task<BlogTag> CreateBlogTagAsync(BlogTag blogTag) => _blogTagDAO.CreateBlogTagAsync(blogTag);

        public Task<BlogTag> DeleteBlogTagAsync(int id) => _blogTagDAO.DeleteBlogTagAsync(id);

        public Task<BlogTag> GetBlogTagByIdAsync(int id) => _blogTagDAO.GetBlogTagByIdAsync(id);

        public Task<IEnumerable<BlogTag>> GetBlogTagsAsync() => _blogTagDAO.GetBlogTagsAsync();

        public Task<BlogTag> UpdateBlogTagAsync(BlogTag blogTag) => _blogTagDAO.UpdateBlogTagAsync(blogTag);
    }
}
