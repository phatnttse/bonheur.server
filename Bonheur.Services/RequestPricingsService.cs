using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.RequestPricing;
using Bonheur.Services.DTOs.Supplier;
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
    public class RequestPricingsService : IRequestPricingsService
    {
        private readonly IRequestPricingsRepository _requestPricingsRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;
        public RequestPricingsService(IRequestPricingsRepository requestPricingsRepository, IUserAccountRepository userAccountRepository, IMapper mapper, ISupplierRepository supplierRepository)
        {
            _requestPricingsRepository = requestPricingsRepository;
            _userAccountRepository = userAccountRepository;
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }
        public async Task<ApplicationResponse> CreateRequestPricing(CreateRequestPricingDTO requestPricingDTO)
        {
            try
            {
                var requestPricing = _mapper.Map<RequestPricing>(requestPricingDTO);
                if (requestPricing == null)
                {
                    throw new ApiException("Request pricing is not exist!");
                }
                var supplier = _supplierRepository.GetSupplierByIdAsync(requestPricing.SupplierId, false);
                if (supplier == null) {
                    throw new ApiException("Supplier was not found!");
                }
                await _requestPricingsRepository.CreateRequestPricing(requestPricing);
                return new ApplicationResponse
                {
                    Message = "Request pricing create successfully!",
                    Success = true,
                    Data = requestPricingDTO,
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

        public async Task<ApplicationResponse> GetAllRequestPricing(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var requestPricingPagedList = await _requestPricingsRepository.GetAllRequestPricing(pageNumber, pageSize);
                var requestPricingsDTO = _mapper.Map<List<RequestPricingsDTO>>(requestPricingPagedList);

                var responseData = new PagedData<RequestPricingsDTO>
                {
                    Items = requestPricingsDTO,
                    PageNumber = requestPricingPagedList.PageNumber,
                    PageSize = requestPricingPagedList.PageSize,
                    TotalItemCount = requestPricingPagedList.TotalItemCount,
                    PageCount = requestPricingPagedList.PageCount,
                    IsFirstPage = requestPricingPagedList.IsFirstPage,
                    IsLastPage = requestPricingPagedList.IsLastPage,
                    HasNextPage = requestPricingPagedList.HasNextPage,
                    HasPreviousPage = requestPricingPagedList.HasPreviousPage

                };

                return new ApplicationResponse
                {
                    Message = "Request pricing query successfully!",
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

        public async Task<ApplicationResponse> GetAllRequestPricingBySupplierId(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                var supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);
                
                var requestPricingPagedList = await _requestPricingsRepository.GetAllRequestPricingBySupplierId(supplier.Id,pageNumber, pageSize);
                var requestPricingsDTO = _mapper.Map<List<RequestPricingsDTO>>(requestPricingPagedList);

                var responseData = new PagedData<RequestPricingsDTO>
                {
                    Items = requestPricingsDTO,
                    PageNumber = requestPricingPagedList.PageNumber,
                    PageSize = requestPricingPagedList.PageSize,
                    TotalItemCount = requestPricingPagedList.TotalItemCount,
                    PageCount = requestPricingPagedList.PageCount,
                    IsFirstPage = requestPricingPagedList.IsFirstPage,
                    IsLastPage = requestPricingPagedList.IsLastPage,
                    HasNextPage = requestPricingPagedList.HasNextPage,
                    HasPreviousPage = requestPricingPagedList.HasPreviousPage

                };

                return new ApplicationResponse
                {
                    Message = "Request pricing query successfully!",
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

        public async Task<ApplicationResponse> GetRequestPricingById(int id)
        {
            try
            {
                var requestPricing = await _requestPricingsRepository.GetRequestPricingById(id);
                if (requestPricing == null)
                {
                    throw new ApiException("Request Pricing does not exist!", HttpStatusCode.NotFound);
                }
                return new ApplicationResponse
                {
                    Message = "Request pricing query successfully!",
                    Success = true,
                    Data = requestPricing,
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

        public async Task<ApplicationResponse> UpdateRequestPricingStatus(int id, RequestPricingStatus status)
        {
            try
            {
                var requestPricingExist = await _requestPricingsRepository.GetRequestPricingById(id);

                if (requestPricingExist == null)
                {
                    throw new ApiException("Request Pricing does not exist!");
                }

                if (!Enum.IsDefined(typeof(RequestPricingStatus), status))
                {
                    throw new ApiException("Invalid Request Pricing Status!", HttpStatusCode.BadRequest);
                }

                requestPricingExist.Status = status;
                await _requestPricingsRepository.UpdateRequestPricingStatus(requestPricingExist);

                return new ApplicationResponse
                {
                    Message = "Request pricing status update successfully!",
                    Success = true,
                    Data = requestPricingExist,
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
