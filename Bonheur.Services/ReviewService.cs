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
                var reviewPagedList = await _reviewRepository.GetReviews(supplierId, pageNumber, pageSize);
                var reviewsDTO = _mapper.Map<List<ReviewDTO>>(reviewPagedList);

                var responseData = new PagedData<ReviewDTO>
                {
                    Items = reviewsDTO,
                    PageNumber = reviewPagedList.PageNumber,
                    PageSize = reviewPagedList.PageSize,
                    TotalItemCount = reviewPagedList.TotalItemCount,
                    PageCount = reviewPagedList.PageCount,
                    IsFirstPage = reviewPagedList.IsFirstPage,
                    IsLastPage = reviewPagedList.IsLastPage,
                    HasNextPage = reviewPagedList.HasNextPage,
                    HasPreviousPage = reviewPagedList.HasPreviousPage

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
