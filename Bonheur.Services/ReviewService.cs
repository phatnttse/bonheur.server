using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.DAOs;
using Bonheur.Repositories;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.RequestPricing;
using Bonheur.Services.DTOs.Review;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;
        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, ISupplierRepository supplierRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _supplierRepository = supplierRepository;
        }

        public async Task<ApplicationResponse> AddNewReview(CreateUpdateReviewDTO reviewDTO)
        {
            try
            {
                var review = _mapper.Map<Review>(reviewDTO);
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);
                var supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);
                review.UserId = currentUserId;
                if (reviewDTO == null)
                {
                    throw new ApiException("Review does not exist!");
                }
                if(review.SupplierId == supplier?.Id)
                {
                    throw new ApiException("Fobidden: You are reviewing yourself");
                }
                await _reviewRepository.AddNewReview(review);
                return new ApplicationResponse
                {
                    Message = "Review create successfully!",
                    Success = true,
                    Data = reviewDTO,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCode.InternalServerError);
            }

        }

        public async Task<ApplicationResponse> GetReviews(int supplierId, int pageNumber, int pageSize)
        {
            try
            {
                var reviewData = await _reviewRepository.GetReviews(supplierId, pageNumber, pageSize);

                var reviewPagedList = reviewData.GetType().GetProperty("Reviews")?.GetValue(reviewData) as IPagedList<Review>;

                var reviewsDTO = _mapper.Map<List<ReviewDTO>>(reviewPagedList);

                var averageScores = new
                {
                    AverageValueOfMoney = reviewData.GetType().GetProperty("AvgValueOfMoney")?.GetValue(reviewData) ?? 0,
                    AverageFlexibility = reviewData.GetType().GetProperty("AvgFlexibility")?.GetValue(reviewData) ?? 0,
                    AverageProfessionalism = reviewData.GetType().GetProperty("AvgProfessionalism")?.GetValue(reviewData) ?? 0,
                    AverageQualityOfService = reviewData.GetType().GetProperty("AvgQualityOfService")?.GetValue(reviewData) ?? 0,
                    AverageResponseTime = reviewData.GetType().GetProperty("AvgResponseTime")?.GetValue(reviewData) ?? 0
                };

                var responseData = new
                {
                    Reviews = new PagedData<ReviewDTO>
                    {
                        Items = reviewsDTO,
                        PageNumber = reviewPagedList?.PageNumber ?? 1,
                        PageSize = reviewPagedList?.PageSize ?? 10,
                        TotalItemCount = reviewPagedList?.TotalItemCount ?? 0,
                        PageCount = reviewPagedList?.PageCount ?? 0,
                        IsFirstPage = reviewPagedList?.IsFirstPage ?? false,
                        IsLastPage = reviewPagedList?.IsLastPage ?? false,
                        HasNextPage = reviewPagedList?.HasNextPage ?? false,
                        HasPreviousPage = reviewPagedList?.HasPreviousPage ?? false
                    },
                    AverageScores = averageScores
                };

                return new ApplicationResponse
                {
                    Message = "Review query successfully!",
                    Success = true,
                    Data = responseData,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCode.InternalServerError);
            }
        }


        public async Task<ApplicationResponse> GetReviewById(int id)
        {
            try
            {
                var review = await _reviewRepository.GetReview(id);

                return new ApplicationResponse
                {
                    Message = $"Review with id:{id} query successfully!",
                    Success = true,
                    Data = review,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new ApiException(ex.Message, HttpStatusCode.InternalServerError);
            }

        }

        public async Task<ApplicationResponse> DeleteReview(int id)
        {
            try
            {

                var review = await _reviewRepository.GetReview(id);
                if (review == null)
                {
                    throw new ApiException("Review does not exist!");
                }
                await _reviewRepository.DeleteReview(id); 
                return new ApplicationResponse
                {
                    Message = $"Review with id:{id} delete successfully!",
                    Success = true,
                    Data = review,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (ApiException)
            {

                throw;
            }
            catch(Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> UpdateReview(CreateUpdateReviewDTO reviewDTO, int id)
        {
            try
            {
                var reviewExist = await _reviewRepository.GetReview(id);

                if (reviewExist == null)
                {
                    throw new ApiException("Review does not exist!");
                }

                var updateReview = _mapper.Map(reviewDTO, reviewExist);
                await _reviewRepository.UpdateReview(updateReview);

                return new ApplicationResponse
                {
                    Message = "Update review successfully!",
                    Success = true,
                    Data = updateReview,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (ApiException)
            {

                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

    }
}
