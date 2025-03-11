using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Storage;
using Bonheur.Services.DTOs.Supplier;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Http;


namespace Bonheur.Services
{
    public class SupplierVideoService : ISupplierVideoService
    {
        private readonly ISupplierVideoRepository _supplierVideoRepository;
        private readonly IStorageService _storageService;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierVideoService(ISupplierVideoRepository supplierVideoRepository, IStorageService storageService, ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierVideoRepository = supplierVideoRepository;
            _storageService = storageService;
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<ApplicationResponse> AddSupplierVideoAsync(IFormFile file)
        {
            try
            {
                if (!Utilities.IsValidVideo(file)) throw new ApiException("Invalid video format", System.Net.HttpStatusCode.BadRequest);

                if (!Utilities.IsValidSizeFile(file, 500 * 1024 * 1024)) throw new ApiException("File size too large. Maximum file size is 500MB", System.Net.HttpStatusCode.BadRequest);

                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                Supplier? supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);

                if (supplier == null) throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);
              
                AzureBlobResponseDTO response = await _storageService.UploadAsync(file);

                SupplierVideo supplierVideo = new SupplierVideo
                {
                    SupplierId = supplier.Id,
                    Url = response.Blob.Uri,
                    FileName = response.Blob.Name,
                    VideoType = file.ContentType                   
                };

                await _supplierVideoRepository.AddSupplierVideoAsync(supplierVideo);

                return new ApplicationResponse
                {
                    Data = _mapper.Map<SupplierVideoDTO>(supplierVideo),
                    Message = "Video uploaded successfully",
                    Success = true,
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

        public async Task<ApplicationResponse> AddSupplierVideoRangeAsync(IEnumerable<IFormFile> files)
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                Supplier? supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);

                if (supplier == null) throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                List<SupplierVideo> supplierVideos = new List<SupplierVideo>();

                foreach (var file in files)
                {
                    AzureBlobResponseDTO response = await _storageService.UploadAsync(file);

                    SupplierVideo supplierVideo = new SupplierVideo
                    {
                        SupplierId = supplier.Id,
                        Url = response.Blob.Uri,
                        FileName = file.FileName,
                        VideoType = file.ContentType
                    };

                    supplierVideos.Add(supplierVideo);
                }

                await _supplierVideoRepository.AddSupplierVideoRangeAsync(supplierVideos);

                return new ApplicationResponse
                {
                    Data = _mapper.Map<List<SupplierVideoDTO>>(supplierVideos),
                    Message = "Videos uploaded successfully",
                    Success = true,
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

        public async Task<ApplicationResponse> DeleteSupplierVideoAsync(int id)
        {
            try
            {
                SupplierVideo supplierVideo = _supplierVideoRepository.GetSupplierVideoById(id).Result ?? throw new ApiException("Supplier video not found", System.Net.HttpStatusCode.NotFound);

                if (supplierVideo.FileName != null)
                {
                   await _storageService.DeleteAsync(supplierVideo.FileName);
                }

                await _supplierVideoRepository.DeleteSupplierVideoAsync(supplierVideo);

                return new ApplicationResponse
                {
                    Data = _mapper.Map<SupplierVideoDTO>(supplierVideo),
                    Message = "Video deleted successfully",
                    Success = true,
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

        public async Task<ApplicationResponse> GetSupplierVideosBySupplierAsync()
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                Supplier? supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);

                if (supplier == null) throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                IEnumerable<SupplierVideo> supplierVideos = await _supplierVideoRepository.GetSupplierVideosBySupplierIdAsync(supplier.Id);

                return new ApplicationResponse
                {
                    Data = _mapper.Map<List<SupplierVideoDTO>>(supplierVideos),
                    Message = "Videos retrieved successfully",
                    Success = true,
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
