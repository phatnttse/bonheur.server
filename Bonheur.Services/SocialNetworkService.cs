using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Storage;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Http;


namespace Bonheur.Services
{
    public class SocialNetworkService : ISocialNetworkService
    {
        private readonly ISocialNetworkRepository _socialNetworkRepository;
        private readonly IStorageService _storageService;

        public SocialNetworkService(ISocialNetworkRepository socialNetworkRepository, IStorageService storageService)
        {
            _socialNetworkRepository = socialNetworkRepository;
            _storageService = storageService;
        }

        public async Task<ApplicationResponse> CreateSocialNetwork(IFormFile file, string name)
        {
            try
            {
                AzureBlobResponseDTO uploadResponse = await _storageService.UploadAsync(file);

                if (uploadResponse.Error) throw new ApiException(uploadResponse?.Status!, System.Net.HttpStatusCode.BadRequest);

                SocialNetwork socialNetwork = new SocialNetwork
                {
                    Name = name,
                    ImageUrl = uploadResponse.Blob.Uri,
                    ImageFileName = uploadResponse.Blob.Name
                };

                SocialNetwork newSocialNetwork = await _socialNetworkRepository.CreateAsync(socialNetwork);

                return new ApplicationResponse
                {
                    Message = "Social network created successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                    Data = newSocialNetwork
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

        public async Task<ApplicationResponse> DeleteSocialNetwork(int id)
        {
            try
            {
               SocialNetwork existingSocialNetwork = await _socialNetworkRepository.GetByIdAsync(id) ?? throw new ApiException("Social network not found", System.Net.HttpStatusCode.NotFound);

                await _socialNetworkRepository.DeleteAsync(existingSocialNetwork);

                if (!string.IsNullOrEmpty(existingSocialNetwork?.ImageFileName!))
                {
                    await _storageService.DeleteAsync(existingSocialNetwork.ImageFileName);
                }

                return new ApplicationResponse
                {
                    Data = existingSocialNetwork,
                    Message = "Social network deleted successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true
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

        public async Task<ApplicationResponse> GetSocialNetworkById(int id)
        {
            try
            {
               SocialNetwork existingSocialNetwork = await _socialNetworkRepository.GetByIdAsync(id) ?? throw new ApiException("Social network not found", System.Net.HttpStatusCode.NotFound);

                return new ApplicationResponse
                {
                    Message = "Social network retrieved successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                    Data = existingSocialNetwork
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

        public async Task<ApplicationResponse> GetSocialNetworks()
        {
            try
            {
                IEnumerable<SocialNetwork> socialNetworks = await _socialNetworkRepository.GetAllAsync();

                return new ApplicationResponse
                {
                    Message = "Social networks retrieved successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                    Data = socialNetworks
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

        public async Task<ApplicationResponse> UpdateSocialNetwork(IFormFile file, string name, int id)
        {
            try
            {
                SocialNetwork existingSocialNetwork = await _socialNetworkRepository.GetByIdAsync(id) ?? throw new ApiException("Social network not found", System.Net.HttpStatusCode.NotFound);

                if (file != null)
                {
                    string currentFileName = existingSocialNetwork?.ImageFileName!;

                    AzureBlobResponseDTO uploadResponse = await _storageService.UploadAsync(file);

                    if (uploadResponse.Error) throw new ApiException(uploadResponse?.Status!, System.Net.HttpStatusCode.BadRequest);

                    existingSocialNetwork!.ImageUrl = uploadResponse.Blob.Uri;
                    existingSocialNetwork.ImageFileName = uploadResponse.Blob.Name;

                    if (!string.IsNullOrEmpty(currentFileName))
                    {
                        await _storageService.DeleteAsync(currentFileName);
                    }
                }

                existingSocialNetwork.Name = name;

                SocialNetwork newSocialNetwork = await _socialNetworkRepository.UpdateAsync(existingSocialNetwork);

                return new ApplicationResponse
                {
                    Message = "Social network updated successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                    Data = newSocialNetwork
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
