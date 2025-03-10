using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogPostDAO _blogPostDAO;
        public BlogPostRepository(BlogPostDAO blogPostDAO)
        {
            _blogPostDAO = blogPostDAO;
        }
        public Task AddBlogPostAsync(BlogPost blogPost) => _blogPostDAO.CreateBlogPostAsync(blogPost);

        public Task DeleteBlogPostAsync(BlogPost blogPost) => _blogPostDAO.DeleteBlogPostAsync(blogPost.Id);


        public Task<BlogPost?> GetBlogPostByIdAsync(int id) => _blogPostDAO.GetBlogPostByIdAsync(id);

        public Task<IPagedList<BlogPost>> GetBlogPostsAsync(string? searchTitle, string? searchContent, int pageNumber = 1, int pageSize = 10) => _blogPostDAO.GetBlogPostsAsync(searchTitle, searchContent, pageNumber, pageSize);

        public Task<IPagedList<BlogPost>> GetBlogPostsByTagsAsync(List<BlogTag> tags, int pageNumber = 1, int pageSize = 10) => _blogPostDAO.GetBlogPostsByTags(tags, pageNumber, pageSize);

        public Task UpdateBlogPostAsync(BlogPost blogPost) => _blogPostDAO.UpdateBlogPostAsync(blogPost);
    }
}
