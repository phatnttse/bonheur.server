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
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public FavoriteSupplierService(
            IFavoriteSupplierRepository favoriteSupplierRepository,
            IMapper mapper,
            ISupplierRepository supplierRepository)
        {
            _favoriteSupplierRepository = favoriteSupplierRepository;
            _mapper = mapper;
            _supplierRepository = supplierRepository;
        }

        public async Task<ApplicationResponse> AddFavoriteSupplier(int supplierId)
        {
            try
            {
                string userId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);
                FavoriteSupplierDTO favoriteSupplierDTO = new FavoriteSupplierDTO();
                var existedSupplier = await _supplierRepository.GetSupplierByIdAsync(supplierId, false);
                if (existedSupplier == null) {
                    throw new ApiException("Supplier was not existed!", System.Net.HttpStatusCode.BadRequest);
                }
                var checkSupplier = await _supplierRepository.GetSupplierByUserIdAsync(userId);

                if (checkSupplier != null)
                {
                    if (checkSupplier!.Id == supplierId) throw new ApiException("You are loving yourself!", System.Net.HttpStatusCode.BadRequest);                 
                }

                var existedFavoriteSupplier = await _favoriteSupplierRepository.GetFavoriteSupplierAsync(supplierId);
                if (existedFavoriteSupplier != null) {
                    throw new ApiException("The supplier already exists in the favorites list!", System.Net.HttpStatusCode.BadRequest);
                }
                favoriteSupplierDTO.UserId = userId;
                favoriteSupplierDTO.SupplierId = supplierId;
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
                    Data = favoriteSupplier,
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

        public async Task<ApplicationResponse> GetFavoriteSupplierCountByCategoryAsync()
        {
            try
            {
                var favoriteCounts = await _favoriteSupplierRepository.GetFavoriteSupplierCountByCategoryAsync();


                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Query favorite supplier count by category successfully",
                    Data = favoriteCounts,
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


        public async Task<ApplicationResponse> GetFavoriteSuppliersByCategoryId(int categoryId, int pageNumber, int pageSize)
        {
            try
            {
                string userId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);
                var favoriteSupplier = await _favoriteSupplierRepository.GetFavoriteSuppliersByCategoryId(userId, categoryId, pageNumber, pageSize);
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
                    Message = "Query all favorite supplier by categoryId successfully",
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

    }
}
