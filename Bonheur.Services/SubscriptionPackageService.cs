using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.SubscriptionPackage;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;


namespace Bonheur.Services
{
    public class SubscriptionPackageService : ISubscriptionPackageService
    {
        private readonly ISubscriptionPackageRepository _subscriptionPackageRepository;
        private readonly IMapper _mapper;

        public SubscriptionPackageService(ISubscriptionPackageRepository subscriptionPackageRepository, IMapper mapper)
        {
            _subscriptionPackageRepository = subscriptionPackageRepository;
            _mapper = mapper;
        }

        public async Task<ApplicationResponse> GetAllSubscriptionPackagesAsync()
        {
            try
            {
                var subscriptionPackages = await _subscriptionPackageRepository.GetAllSubscriptionPackagesAsync();

                return new ApplicationResponse
                {
                    Message = "Get supscription packages successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                    Data = _mapper.Map<List<SubscriptionPackageDTO>>(subscriptionPackages),
                };
            }
            catch (ApiException)
            {
                throw;

            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetSubscriptionPackageByIdAsync(int id)
        {
            try
            {
                var subscriptionPackage = await _subscriptionPackageRepository.GetSubscriptionPackageByIdAsync(id);

                if (subscriptionPackage == null) throw new ApiException("Subscription package not found", System.Net.HttpStatusCode.NotFound);

                return new ApplicationResponse
                {
                    Message = "Get supscription package successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                    Data = _mapper.Map<SubscriptionPackageDTO>(subscriptionPackage),
                };
            }
            catch (ApiException)
            {
                throw;

            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> CreateSubscriptionPackageAsync(CreateSubscriptionPackageDTO subscriptionPackageDTO)
        {
            try
            {
                var subscriptionPackage = _mapper.Map<SubscriptionPackage>(subscriptionPackageDTO);

                await _subscriptionPackageRepository.CreateSubscriptionPackageAsync(subscriptionPackage);

                return new ApplicationResponse
                {
                    Message = "Create supscription package successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = _mapper.Map<CreateSubscriptionPackageDTO>(subscriptionPackage),
                    Success = true,
                };
            }
            catch (ApiException)
            {
                throw;

            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> UpdateSubscriptionPackageAsync(int id, SubscriptionPackageDTO subscriptionPackageDTO)
        {
            try
            {
                var subscriptionPackage = await _subscriptionPackageRepository.GetSubscriptionPackageByIdAsync(id);

                if (subscriptionPackage == null) throw new ApiException("Subscription package not found", System.Net.HttpStatusCode.NotFound);

                _mapper.Map(subscriptionPackageDTO, subscriptionPackage);

                await _subscriptionPackageRepository.UpdateSubscriptionPackageAsync(subscriptionPackage);

                return new ApplicationResponse
                {
                    Message = "Update supscription package successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = _mapper.Map<SubscriptionPackageDTO>(subscriptionPackage),
                    Success = true,
                };
            }
            catch (ApiException)
            {
                throw;

            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> DeleteSubscriptionPackageAsync(int id)
        {
            try
            {
                var subscriptionPackage = await _subscriptionPackageRepository.GetSubscriptionPackageByIdAsync(id);

                if (subscriptionPackage == null) throw new ApiException("Subscription package not found", System.Net.HttpStatusCode.NotFound);

                subscriptionPackage.IsDeleted = true;

                await _subscriptionPackageRepository.UpdateSubscriptionPackageAsync(subscriptionPackage);

                return new ApplicationResponse
                {
                    Message = "Delete supscription package successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = _mapper.Map<SubscriptionPackageDTO>(subscriptionPackage),
                    Success = true,
                };
            }
            catch (ApiException)
            {
                throw;

            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

    }
}
