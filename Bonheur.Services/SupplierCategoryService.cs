using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.SupplierCategory;
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
    public class SupplierCategoryService : ISupplierCategoryService
    {
        private readonly ISupplierCategoryRepository _supplierCategoryRepository;
        private readonly IMapper _mapper;
        public SupplierCategoryService(ISupplierCategoryRepository supplierCategoryRepository, IMapper mapper)
        {
            _supplierCategoryRepository = supplierCategoryRepository;
            _mapper = mapper;
        }

        public async Task<ApplicationResponse> AddSupplierCategory(CreateSupplierCategoryDTO supplierCategoryDTO)
        {
            try
            {
                var supplierCategory = _mapper.Map<SupplierCategory>(supplierCategoryDTO);
                await _supplierCategoryRepository.AddSupplierCategory(supplierCategory);
                return new ApplicationResponse
                {
                    Message = "Supplier category create successfully!",
                    Success = true,
                    Data = supplierCategory,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> DeleteSupplierCategory(int id)
        {
            try
            { 
                var supplierCategory = await _supplierCategoryRepository.GetSupplierCategoryByIdAsync(id);
                if (supplierCategory == null) { 
                    throw new ApiException("Supplier category not found!", HttpStatusCode.InternalServerError);
                }
                await _supplierCategoryRepository.DeleteSupplierCategory(id);
                return new ApplicationResponse
                {
                    Message = "Supplier category deleted successfully",
                    Success = true,
                    Data = supplierCategory,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {

                throw new ApiException(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetAllSupplierCategoryAsync()
        {
            try
            {
                var supplierCategories = await _supplierCategoryRepository.GetAllSupplierCategoryAsync();
                if (!supplierCategories.Any()) {
                    return new ApplicationResponse
                    {
                        Success = true,
                        Message = "No supplier category found!",
                        Data = new List<SupplierCategoryDTO>(),
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                var supplierCategoryDTOs = _mapper.Map<List<SupplierCategoryDTO>>(supplierCategories);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Supplier categories retrieved successfully",
                    Data = supplierCategoryDTOs,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {

                throw new ApiException(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetSupplierCategoryByIdAsync(int id)
        {
            try
            {
                var supplierCategory = await _supplierCategoryRepository.GetSupplierCategoryByIdAsync(id);
                if (supplierCategory == null) {
                    return new ApplicationResponse
                    {
                        Success = true,
                        Message = "No supplier category was found!",
                        Data = null,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                var supplierCategoryDTO = _mapper.Map<SupplierCategoryDTO>(supplierCategory);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Supplier category retrieved successfully",
                    Data = supplierCategoryDTO,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> UpdateSupplierCategory(CreateSupplierCategoryDTO supplierCategoryDTO, int id)
        {
            try
            {
                if (supplierCategoryDTO == null)
                {
                    throw new ApiException("Invalid input data", HttpStatusCode.BadRequest);
                }

                var existingSupplierCategory = await _supplierCategoryRepository.GetSupplierCategoryByIdAsync(id);

                if (existingSupplierCategory == null)
                {
                    return new ApplicationResponse
                    {
                        Success = false,
                        Message = "Supplier category not found",
                        Data = null,
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                existingSupplierCategory.Name = supplierCategoryDTO.Name;
                existingSupplierCategory.Description = supplierCategoryDTO.Description;

                await _supplierCategoryRepository.UpdateSupplierCategory(existingSupplierCategory);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Supplier category updated successfully",
                    Data = supplierCategoryDTO, 
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {

                throw new ApiException(ex.Message, HttpStatusCode.InternalServerError);

            }
        }
    }
}
