using Bonheur.BusinessObjects.Entities;
using Microsoft.AspNetCore.DataProtection.Repositories;
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

        public async Task<object> GetReviewsBySupplierIdPaginated(int supplierId, int pageNumber, int pageSize)
        {
            IQueryable<Review> query = _context.Reviews.Include(rv => rv.User).Where(rv => rv.SupplierId == supplierId)
                .OrderByDescending(rv => rv.CreatedAt);
            var pageList =  query.ToPagedList<Review>(pageNumber, pageSize);
            var averages = await _context.Reviews
                .Where(rv => rv.SupplierId == supplierId)
                .GroupBy(rv => rv.SupplierId)
                .Select(g => new
                {
                    AvgValueOfMoney = g.Average(rv => (double?)rv.ValueForMoney) ?? 0,
                    AvgFlexibility = g.Average(rv => (double?)rv.Flexibility) ?? 0,
                    AvgProfessionalism = g.Average(rv => (double?)rv.Professionalism) ?? 0,
                    AvgQualityOfService = g.Average(rv => (double?)rv.QualityOfService) ?? 0,
                    AvgResponseTime = g.Average(rv => (double?)rv.ResponseTime) ?? 0,
                }).FirstOrDefaultAsync();

            var result = new
            {
                AvgValueOfMoney = averages?.AvgValueOfMoney ?? 0,
                AvgFlexibility = averages?.AvgFlexibility ?? 0,
                AvgProfessionalism = averages?.AvgProfessionalism ?? 0,
                AvgQualityOfService = averages?.AvgQualityOfService ?? 0,
                AvgResponseTime = averages?.AvgResponseTime ?? 0,
                Reviews = pageList
            };
            return result;
        }

        public async Task<object> GetReviewsAverage(int supplierId)
        { 
            var averages = await _context.Reviews
                .Where(rv => rv.SupplierId == supplierId)
                .GroupBy(rv => rv.SupplierId)
                .Select(g => new
                {
                    AvgValueOfMoney = g.Average(rv => (double?)rv.ValueForMoney) ?? 0,
                    AvgFlexibility = g.Average(rv => (double?)rv.Flexibility) ?? 0,
                    AvgProfessionalism = g.Average(rv => (double?)rv.Professionalism) ?? 0,
                    AvgQualityOfService = g.Average(rv => (double?)rv.QualityOfService) ?? 0,
                    AvgResponseTime = g.Average(rv => (double?)rv.ResponseTime) ?? 0,
                }).FirstOrDefaultAsync();

            var result = new
            {
                AvgValueOfMoney = averages?.AvgValueOfMoney ?? 0,
                AvgFlexibility = averages?.AvgFlexibility ?? 0,
                AvgProfessionalism = averages?.AvgProfessionalism ?? 0,
                AvgQualityOfService = averages?.AvgQualityOfService ?? 0,
                AvgResponseTime = averages?.AvgResponseTime ?? 0,
            };
            return result;
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
