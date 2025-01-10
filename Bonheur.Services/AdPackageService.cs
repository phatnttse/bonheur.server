using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.AdPackage;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services
{
    public class AdPackageService : IAdPackageService
    {
        private readonly IAdPackageRepository _adPackageRepository;
        private readonly IMapper _mapper;
        public AdPackageService(
            IAdPackageRepository adPackageRepository,
            IMapper mapper
            )
        {
            _adPackageRepository = adPackageRepository;
            _mapper = mapper;
        }
        public async Task<ApplicationResponse> AddAdPackage(AdPackageDTO adPackageDTO)
        {
            try
            {
                var adPackage = _mapper.Map<AdPackage>(adPackageDTO);
                await _adPackageRepository.AddAdPackage(adPackage);
                return new ApplicationResponse { 
                    Success = true,
                    Message = "Ad Package was added successfully",
                    Data = adPackage,
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

        public async Task<ApplicationResponse> DeleteAdPackage(int id)
        {
            try
            {
                var adPackage = await _adPackageRepository.GetAdPackageById(id);
                if (adPackage == null) {
                    throw new ApiException("Ad Package does not exist!");
                } 
                await _adPackageRepository.DeleteAdPackage(adPackage);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Delete ad package successfully!",
                    Data = adPackage,
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

        public async Task<ApplicationResponse> GetAdPackageById(int id)
        {
            try
            {
                var adPackage = await _adPackageRepository.GetAdPackageById(id);
                if (adPackage == null) {
                    throw new ApiException("Ad Package does not found!");
                }
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Ad Package query successfully!",
                    Data = adPackage,
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

        public async Task<ApplicationResponse> GetAdPackagesAsync(string? adPackageTitle, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var adPackagePagedList = await _adPackageRepository.GetAdPackagesAsync(adPackageTitle, pageNumber, pageSize);
                var listAdPackageDTO = _mapper.Map<List<AdPackageDTO>>(adPackagePagedList);
                var responseData= new PagedData<AdPackageDTO>
                {
                    Items = listAdPackageDTO,
                    PageNumber = adPackagePagedList.PageNumber,
                    PageSize = adPackagePagedList.PageSize,
                    TotalItemCount = adPackagePagedList.TotalItemCount,
                    PageCount = adPackagePagedList.PageCount,
                    IsFirstPage = adPackagePagedList.IsFirstPage,
                    IsLastPage = adPackagePagedList.IsLastPage,
                    HasNextPage = adPackagePagedList.HasNextPage,
                    HasPreviousPage = adPackagePagedList.HasPreviousPage
                };
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Ad Package query successfully!",
                    Data = responseData,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }
            catch (ApiException)
            {

                throw;
            }
            catch (Exception ex) {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> UpdateAdPackage(int id, AdPackageDTO adPackageDTO)
        {
            try
            {
                var adPackage = await _adPackageRepository.GetAdPackageById(id);
                if (adPackage == null)
                {
                    throw new ApiException("Ad Package does not found!");
                }
                _mapper.Map(adPackageDTO, adPackage);
                await _adPackageRepository.UpdateAdPackage(adPackage);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Update ad package successfully!",
                    Data = adPackage,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex) { 
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
