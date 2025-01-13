using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.AdPackage;
using Bonheur.Services.DTOs.FavoriteSupplier;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services
{
    public class FavoriteSupplierService : IFavoriteSupplierService
    {
        private readonly IFavoriteSupplierRepository _favoriteSupplierRepository;
        private readonly IMapper _mapper;

        public FavoriteSupplierService(
            IFavoriteSupplierRepository favoriteSupplierRepository,
            IMapper mapper)
        {
            _favoriteSupplierRepository = favoriteSupplierRepository;
            _mapper = mapper;
        }

        public async Task<ApplicationResponse> AddFavoriteSupplier(FavoriteSupplierDTO favoriteSupplierDTO)
        {
            try
            {
                var favoriteSupplier = _mapper.Map<FavoriteSupplier>(favoriteSupplierDTO);
                await _favoriteSupplierRepository.AddFavoriteSupplier(favoriteSupplier);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Added favorite supplier successfully!",
                    Data = favoriteSupplierDTO,
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

        public async Task<ApplicationResponse> DeleteSupplierAsync(int id)
        {
            try
            {
                var favoriteSupplier = await _favoriteSupplierRepository.GetFavoriteSupplierAsync(id) ?? throw new ApiException("Favorite Supplier was not found!");
                await _favoriteSupplierRepository.DeleteSupplierAsync(favoriteSupplier);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Favorite Supplier was deleted successfully!",
                    Data = null,
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

        public async Task<ApplicationResponse> GetAllFavoriteSuppliers(int pageNumber, int pageSize)
        {
            try
            {
                string userId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);
                var favoriteSupplier = await _favoriteSupplierRepository.GetAllFavoriteSuppliers(userId, pageNumber, pageSize);
                var result = _mapper.Map<List<FavoriteSupplierDTO>>(favoriteSupplier);
                var responseData = new PagedData<FavoriteSupplierDTO>
                {
                    Items = result,
                    PageNumber = favoriteSupplier.PageNumber,
                    PageSize = favoriteSupplier.PageSize,
                    TotalItemCount = favoriteSupplier.TotalItemCount,
                    PageCount = favoriteSupplier.PageCount,
                    IsFirstPage = favoriteSupplier.IsFirstPage,
                    IsLastPage = favoriteSupplier.IsLastPage,
                    HasNextPage = favoriteSupplier.HasNextPage,
                    HasPreviousPage = favoriteSupplier.HasPreviousPage
                };
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Query all favorite supplier successfully",
                    Data = result,
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

        public async Task<ApplicationResponse> GetFavoriteSupplierAsync(int id)
        {
            try
            {
                var favoriteSupplier = await _favoriteSupplierRepository.GetFavoriteSupplierAsync(id);
                var result = _mapper.Map<FavoriteSupplierDTO>(favoriteSupplier);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Query favorite supplier successfully!",
                    Data = result,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> UpdateFavoriteSupplierAsync(int id, FavoriteSupplierDTO favoriteSupplierDTO)
        {
            try
            {
                var existedFavoriteSupplier = await _favoriteSupplierRepository.GetFavoriteSupplierAsync(id) ?? throw new ApiException("Favorite supplier was not found!");
                _mapper.Map(existedFavoriteSupplier, favoriteSupplierDTO);
                await _favoriteSupplierRepository.UpdateFavoriteSupplierAsync(existedFavoriteSupplier);
                var result = _mapper.Map<FavoriteSupplierDTO>(existedFavoriteSupplier);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Update favorite supplier successfully!",
                    Data = result,
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
    }
}
