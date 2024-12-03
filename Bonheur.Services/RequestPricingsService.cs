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
        private readonly ISupplierRepository _supplierRepository;
        private readonly IUserAccountRepository _userAccountRepository;
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
                if (requestPricing == null) {
                    throw new ApiException("Request pricing is not exist!");
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
            catch(ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetAllRequestPricing()
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);


                var currentUser = await _userAccountRepository.GetUserByIdAsync(currentUserId);

                if (currentUser == null) throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                if(await _supplierRepository.IsSupplierAsync(currentUserId) == false)
                {
                    throw new ApiException("Your are not the supplier!");
                }

                var listRequestPricing = await _requestPricingsRepository.GetAllRequestPricing(currentUserId);

                return new ApplicationResponse
                {
                    Message = "Request pricing query successfully!",
                    Success = true,
                    Data = listRequestPricing,
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
                if(requestPricing == null)
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

                if (requestPricingExist == null) {
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
