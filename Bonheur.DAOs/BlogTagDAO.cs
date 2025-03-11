using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.DAOs
{
    public class BlogTagDAO
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public BlogTagDAO(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IEnumerable<BlogTag>> GetBlogTagsAsync()
        {
            return await _applicationDbContext.BlogTags.ToListAsync();
        }

        public async Task<BlogTag> GetBlogTagByIdAsync(int id)
        {
            return await _applicationDbContext.BlogTags.FindAsync(id);
        }

        public async Task<BlogTag> CreateBlogTagAsync(BlogTag blogTag)
        {
            _applicationDbContext.BlogTags.Add(blogTag);
            await _applicationDbContext.SaveChangesAsync();
            return blogTag;
        }

        public async Task<BlogTag> UpdateBlogTagAsync(BlogTag blogTag)
        {
            _applicationDbContext.BlogTags.Update(blogTag);
            await _applicationDbContext.SaveChangesAsync();
            return blogTag;
        }

        public async Task<BlogTag> DeleteBlogTagAsync(int id)
        {
            var blogTag = await _applicationDbContext.BlogTags.FindAsync(id);
            _applicationDbContext.BlogTags.Remove(blogTag);
            await _applicationDbContext.SaveChangesAsync();
            return blogTag;
        }
    }
}
