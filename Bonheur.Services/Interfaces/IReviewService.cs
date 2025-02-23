using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ApplicationResponse> GetReviews(int supplierId, int pageNumber, int pageSize);
        Task<ApplicationResponse> GetReviewById(int id);

        Task<ApplicationResponse> AddNewReview(CreateUpdateReviewDTO reviewDTO);

        Task<ApplicationResponse> UpdateReview(CreateUpdateReviewDTO reviewDTO, int id);

        Task<ApplicationResponse> DeleteReview(int id);
        Task<ApplicationResponse> SendEmailRequestReview(SendEmailReviewDTO sendEmailReviewDTO);

        Task<ApplicationResponse> GetReviewsAverage(int supplierId);

    }
}
