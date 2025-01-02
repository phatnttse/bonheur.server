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
    public class ReviewDAO
    {
        private ApplicationDbContext _context;

        public ReviewDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<IPagedList<Review>> GetReviews(int pageNumber, int pageSize)
        {
            IQueryable<Review> query = _context.Reviews.Include(rv=> rv.User)
            .OrderByDescending(rv => rv.CreatedAt);
            var pageList = query.ToPagedList<Review>(pageNumber, pageSize);
            return Task.FromResult(pageList);
        }

        public Task<Review> GetReview(int id)
        {
            return _context.Reviews.Include(rv => rv.User).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddNewReview(Review newReview)
        {
            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReview(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReview(int id)
        {
            var review = _context.Reviews.FirstOrDefault(x => x.Id == id);
             _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
