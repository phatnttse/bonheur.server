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
    public class ReviewRepository : IReviewRepository
    {
        private readonly ReviewDAO _reviewDAO;
        public ReviewRepository(ReviewDAO reviewDAO)
        {
            _reviewDAO = reviewDAO;
        }
        public Task AddNewReview(Review newReview) => _reviewDAO.AddNewReview(newReview);

        public Task DeleteReview(int id) => _reviewDAO.DeleteReview(id);

        public Task<Review> GetReview(int id) => _reviewDAO.GetReview(id);  


        public Task<IPagedList<Review>> GetReviews(int pageNumber, int pageSize) => _reviewDAO.GetReviews(pageNumber, pageSize);

        public Task UpdateReview(Review review) => _reviewDAO.UpdateReview(review); 
    }
}
