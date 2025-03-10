using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Bonheur.DAOs
{
    public class BlogPostDAO
    {
        private readonly ApplicationDbContext _context;
        public BlogPostDAO(ApplicationDbContext context)
        {
            _context = context; 
        }

        public async Task<IPagedList<BlogPost>> GetBlogPostsAsync(string? searchTitle, string? searchContent, int pageNumber=1, int pageSize = 10)
        {
            IQueryable<BlogPost> query = _context.BlogPosts;
            if (!string.IsNullOrEmpty(searchTitle))
            {
                query = query.Where(a => EF.Functions.Like(a.Title, $"%{a.Title}%"));
            }
            if (!string.IsNullOrEmpty(searchContent))
            {
                query = query.Where(a => EF.Functions.Like(a.Content, $"%{a.Content}%"));
            }
            var orderedQuery = query.OrderByDescending(a => a.CreatedAt);
            var blogPostPaginated = orderedQuery.ToPagedList(pageNumber, pageSize);
            return await Task.FromResult(blogPostPaginated);
        }

        public async Task<IPagedList<BlogPost>> GetBlogPostsByTags(List<BlogTag> tags, int pageNumber = 1, int pageSize = 10)
        {
            IQueryable<BlogPost> query = _context.BlogPosts;

            if (tags != null && tags.Any())
            {
                var tagNames = tags.Select(tag => tag.Name).ToList();
                query = query.Where(a => a.Tags.Any(tag => tagNames.Contains(tag.Name)));
            }

            var orderedQuery = query.OrderByDescending(a => a.CreatedAt);
            var blogPostPaginated = orderedQuery.ToPagedList(pageNumber, pageSize);
            return await Task.FromResult(blogPostPaginated);
        }


        public async Task<BlogPost?> GetBlogPostByIdAsync(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            return await Task.FromResult(blogPost);
        }

        public async Task<BlogPost?> CreateBlogPostAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            return await Task.FromResult(blogPost);
        }

        public async Task<BlogPost?> UpdateBlogPostAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Update(blogPost);
            await _context.SaveChangesAsync();
            return await Task.FromResult(blogPost);
        }

        public async Task<BlogPost?> DeleteBlogPostAsync(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return await Task.FromResult(blogPost);
        }
    }
}
