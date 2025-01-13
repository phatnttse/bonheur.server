using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Advertisement;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private IAdvertisementRepository _advertisementRepository;
        private IMapper _mapper;
        public AdvertisementService(IAdvertisementRepository advertisementRepository
            , IMapper mapper)
        {
            _advertisementRepository = advertisementRepository;
            _mapper = mapper;

        }

        public async Task<ApplicationResponse> AddAdvertisementAsync(AdvertisementDTO advertisementDTO)
        {
            try
            {
                var advertisement = _mapper.Map<Advertisement>(advertisementDTO);
                await _advertisementRepository.AddAdvertisementAsync(advertisement);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Advertisement was added successfully!",
                    Data = advertisementDTO,
                    StatusCode = System.Net.HttpStatusCode.OK
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

        public async Task<ApplicationResponse> DeleteAdvertisementAsync(int id)
        {
            try
            {
                var advertisement = await _advertisementRepository.GetAdvertisementByIdAsync(id) ??
                    throw new ApiException("Advertisement was not found!");
                await _advertisementRepository.DeleteAdvertisementAsync(advertisement);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Advertisement was deleted successfully!",
                    Data = null,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
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

        public async Task<ApplicationResponse> GetAdvertisementByIdAsync(int id)
        {
            try
            {
                var advertisement = await _advertisementRepository.GetAdvertisementByIdAsync(id);
                if (advertisement == null)
                {
                    throw new ApiException("Advertisement was not found!");
                }
                var advertisementDTO = _mapper.Map<AdvertisementDTO>(advertisement);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Advertisement query successfully",
                    Data = advertisementDTO,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
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

        public async Task<ApplicationResponse> GetAdvertisementsAsync(string searchTitle, string searchContent, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var listAdvertisement = await _advertisementRepository.GetAdvertisementsAsync(searchTitle, searchContent, pageNumber, pageSize);
                var listAdvertisementDTO = _mapper.Map<List<AdvertisementDTO>>(listAdvertisement);
                var responseData = new PagedData<AdvertisementDTO> { 
                    Items = listAdvertisementDTO,
                    PageNumber = listAdvertisement.PageNumber,
                    PageSize = listAdvertisement.PageSize,
                    TotalItemCount = listAdvertisement.TotalItemCount,
                    PageCount = listAdvertisement.PageCount,
                    IsFirstPage = listAdvertisement.IsFirstPage,
                    IsLastPage = listAdvertisement.IsLastPage,
                    HasNextPage = listAdvertisement.HasNextPage,
                    HasPreviousPage = listAdvertisement.HasPreviousPage,
                    };
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "List advertisement query successfully",
                    Data = responseData,
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> UpdateAdvertisementAsync(int id, AdvertisementDTO advertisementDTO)
        {
            try
            {
                var existedAdvertisement = await _advertisementRepository.GetAdvertisementByIdAsync(id) ??
                    throw new ApiException("Advertisement was not found!");
                _mapper.Map(existedAdvertisement, advertisementDTO);
                await _advertisementRepository.UpdateAdvertisementAsync(existedAdvertisement);
                var updatedAdvertisement = _mapper.Map<AdvertisementDTO>(existedAdvertisement);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Advertisement was updated successfully!",
                    Data = updatedAdvertisement,
                    StatusCode = System.Net.HttpStatusCode.OK,
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
