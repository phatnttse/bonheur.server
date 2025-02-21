using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Storage;
using Bonheur.Services.DTOs.SupplierCategory;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Http;
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
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        public SupplierCategoryService(ISupplierCategoryRepository supplierCategoryRepository, IMapper mapper, IStorageService storageService)
        {
            _supplierCategoryRepository = supplierCategoryRepository;
            _mapper = mapper;
            _storageService = storageService;
        }

        public async Task<ApplicationResponse> AddSupplierCategory(IFormFile file, string name, string description)
        {
            try
            {
                AzureBlobResponseDTO uploadResponse = await _storageService.UploadAsync(file);
                if (uploadResponse.Error) throw new ApiException(uploadResponse?.Status!, System.Net.HttpStatusCode.BadRequest);

                SupplierCategoryDTO supplierCategoryDTO = new SupplierCategoryDTO
                {
                    Name = name,
                    Description = description,
                    ImageFileName = uploadResponse.Blob.Name,
                    ImageUrl = uploadResponse.Blob.Uri
                };
                var supplierCategory = _mapper.Map<SupplierCategory>(supplierCategoryDTO);
                if (supplierCategory == null) { 
                    throw new ApiException("Supplier category is exist!", HttpStatusCode.BadRequest);
                }
                var result = await _supplierCategoryRepository.AddSupplierCategory(supplierCategory);

                return new ApplicationResponse
                {
                    Message = "Supplier category create successfully!",
                    Success = true,
                    Data = supplierCategory,
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

        public async Task<ApplicationResponse> DeleteSupplierCategory(int id)
        {
            try
            { 
                var supplierCategory = await _supplierCategoryRepository.GetSupplierCategoryByIdAsync(id);
                if (supplierCategory == null) { 
                    throw new ApiException("Supplier category not found!", HttpStatusCode.NotFound);
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
            catch (ApiException)
            {
                throw;

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
                    throw new ApiException("Supplier category not found!", HttpStatusCode.InternalServerError);
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
            catch (ApiException)
            {
                throw;

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
                    throw new ApiException("Supplier category not found!", HttpStatusCode.NotFound);
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

        public async Task<ApplicationResponse> UpdateSupplierCategory(IFormFile file, string name, string description, int id)
        {
            try
            {

                SupplierCategory existingSupplierCategory = await _supplierCategoryRepository.GetSupplierCategoryByIdAsync(id) ?? throw new ApiException("Supplier category was not found!", System.Net.HttpStatusCode.NotFound);

                if (existingSupplierCategory == null)
                {
                    throw new ApiException("Supplier category not found!", HttpStatusCode.NotFound);
                }


                if (file != null)
                {
                    string currentFileName = existingSupplierCategory?.ImageFileName!;

                    AzureBlobResponseDTO uploadResponse = await _storageService.UploadAsync(file);

                    if (uploadResponse.Error) throw new ApiException(uploadResponse?.Status!, System.Net.HttpStatusCode.BadRequest);

                    existingSupplierCategory!.ImageUrl = uploadResponse.Blob.Uri;
                    existingSupplierCategory.ImageFileName = uploadResponse.Blob.Name;

                    if (!string.IsNullOrEmpty(currentFileName))
                    {
                        await _storageService.DeleteAsync(currentFileName);
                    }
                }

                existingSupplierCategory.Name = name;
                existingSupplierCategory.Description = description;

                await _supplierCategoryRepository.UpdateSupplierCategory(existingSupplierCategory);

                var result = _mapper.Map<SupplierCategoryDTO>(existingSupplierCategory);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Supplier category updated successfully",
                    Data = result, 
                    StatusCode = HttpStatusCode.OK
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
