using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.DAOs;
using Bonheur.Repositories;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.RequestPricing;
using Bonheur.Services.DTOs.Review;
using Bonheur.Services.Email;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
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
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, ISupplierRepository supplierRepository, IEmailSender emailSender, IUserAccountRepository userAccountRepository, ILogger<AuthService> logger)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _emailSender = emailSender;
            _supplierRepository = supplierRepository;
            _userAccountRepository = userAccountRepository;
            _logger = logger;
        }

        public async Task<ApplicationResponse> AddNewReview(CreateUpdateReviewDTO reviewDTO)
        {
            try
            {
                decimal averageRating = 0;
                var review = _mapper.Map<Review>(reviewDTO);
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);
                var supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);
                if (supplier == null) {
                    throw new ApiException("Supplier was not found!");
                }
                #region Average rating
                var supplierUpdate = await _supplierRepository.GetSupplierByIdAsync(reviewDTO.SupplierId, false);

                if (supplierUpdate != null)
                {
                    var supplierTotalRating = (supplierUpdate.TotalRating) + 1;

                    double reviewRating = (reviewDTO.QualityOfService + reviewDTO.Flexibility + reviewDTO.Professionalism
                                          + reviewDTO.ValueForMoney + reviewDTO.ResponseTime) / 5.0;


                    averageRating = (decimal) ((supplierUpdate.TotalRating) * (supplierUpdate.AverageRating) + (decimal)reviewRating) / (supplierTotalRating);

                    supplierUpdate.TotalRating = supplierTotalRating;
                    supplierUpdate.AverageRating = averageRating;

                    await _supplierRepository.UpdateSupplierAsync(supplierUpdate);
                }
                #endregion


                review.UserId = currentUserId;
                if (reviewDTO == null)
                {
                    throw new ApiException("Review does not exist!");
                }
                if (review.SupplierId == supplier?.Id)
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
            catch (Exception ex)
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


        public async Task<ApplicationResponse> SendEmailRequestReview(
            SendEmailReviewDTO sendEmailReviewDTO)
        {
            try
            {
                string currentSupplierUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);
                var supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentSupplierUserId);
                var supplierUserInfomation = await _userAccountRepository.GetUserByIdAsync(currentSupplierUserId);

                if (sendEmailReviewDTO == null || string.IsNullOrEmpty(sendEmailReviewDTO.Email))
                {
                    throw new ApiException("Invalid email or customer!");
                }
                var customer = await _userAccountRepository.GetUserByEmailAsync(sendEmailReviewDTO.Email);
                if (customer == null)
                {
                    throw new ApiException("Customer does not existed!");
                }

                var senderName = supplier?.Name;
                var senderEmail = supplierUserInfomation?.Email;
                var host = supplier?.WebsiteUrl;
                var content = sendEmailReviewDTO?.Content;

                var customerName = customer.FullName;
                var customerEmail = customer.Email;

                var param = new Dictionary<string, string?>
                {
                    {"host", host?.ToString()},
                };

                var message = EmailTemplates.GetRequestToReview(customerName, senderName, host, content);

                _ = Task.Run(async () =>
                {
                    (var success, var errorMsg) = await _emailSender.SendEmailAsync(
                        customerName, customerEmail,
                        $"Review Request from {senderName}", message);

                    if (!success) _logger.LogError($"Failed to send review request email to {customerEmail}: {errorMsg}");
                });

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Review request email sent successfully",
                    Data = message,
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

        public async Task<ApplicationResponse> GetReviewsAverage(int supplierId)
        {
            try
            {
                var reviewData = await _reviewRepository.GetReviewsAverage(supplierId);
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
    }
}
