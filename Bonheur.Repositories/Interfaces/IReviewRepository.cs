using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<IPagedList<Review>> GetReviews(int supplierId, int pageNumber, int pageSize);

        Task<Review> GetReview(int id);

        Task AddNewReview(Review newReview);

        Task UpdateReview(Review review);

        Task DeleteReview(int id);
    }

}
