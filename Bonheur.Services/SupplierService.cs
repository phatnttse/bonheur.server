using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Supplier;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Http;


namespace Bonheur.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IStorageService _storageService;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IMapper _mapper;
        private readonly ISupplierImageRepository _supplierImageRepository;
        private readonly ISupplierCategoryRepository _supplierCategoryRepository;

        public SupplierService(ISupplierRepository supplierRepository, IStorageService storageService, IUserAccountRepository userAccountRepository, IMapper mapper, ISupplierImageRepository supplierImageRepository, 
            ISupplierCategoryRepository supplierCategoryRepository)
        {
            _supplierRepository = supplierRepository;
            _storageService = storageService;
            _userAccountRepository = userAccountRepository;
            _mapper = mapper;
            _supplierImageRepository = supplierImageRepository;
            _supplierCategoryRepository = supplierCategoryRepository;
        }

        public async Task<ApplicationResponse> CreateSupplierAsync(CreateSupplierDTO createSupplierDTO)
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                if (await _supplierRepository.IsSupplierAsync(currentUserId)) throw new ApiException("User is already a supplier", System.Net.HttpStatusCode.BadRequest);

                if (await _supplierCategoryRepository.GetSupplierCategoryByIdAsync(createSupplierDTO.CategoryId) == null) throw new ApiException("Category not found", System.Net.HttpStatusCode.NotFound);

                var currentUser = await _userAccountRepository.GetUserByIdAsync(currentUserId);

                if (currentUser == null) throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);
                
                var supplier = _mapper.Map<Supplier>(createSupplierDTO);

                supplier.UserId = currentUserId;

                var createdSupplier = await _supplierRepository.CreateSupplierAsync(supplier);

                if (createdSupplier == null) throw new ApiException("Failed to create supplier", System.Net.HttpStatusCode.InternalServerError);

                currentUser.PhoneNumber = createSupplierDTO.PhoneNumber;

                var updateUserResult = await _userAccountRepository.UpdateUserAndUserRoleAsync(currentUser, new string[] { Constants.Roles.SUPPLIER });

                if (!updateUserResult.Succeeded) throw new ApiException(string.Join("; ", updateUserResult.Errors.Select(error => error)), System.Net.HttpStatusCode.InternalServerError);

                return new ApplicationResponse
                {
                    Message = "Sign up to become a successful supplier",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                    Data = _mapper.Map<SupplierDTO>(createdSupplier),
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

        public async Task<ApplicationResponse> GetSupplierByIdAsync(int id)
        {
            try
            {
                bool isIncludeUser = false;

                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                if (currentUserId != null)
                {
                    var userAndRoles = await _userAccountRepository.GetUserAndRolesAsync(currentUserId);

                    if (userAndRoles.Value.Roles.Contains(Constants.Roles.ADMIN))
                    {
                        isIncludeUser = true;
                    }

                }

                var supplier = await _supplierRepository.GetSupplierByIdAsync(id, isIncludeUser);

                if (supplier == null) throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                return new ApplicationResponse
                {
                    Message = $"Supplier {supplier.Name} found",
                    Data = _mapper.Map<SupplierDTO>(supplier),
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

        public async Task<ApplicationResponse> GetSupplierByUserIdAsync(string userId)
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                var supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);

                if (supplier == null) throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                return new ApplicationResponse
                {
                    Message = $"Supplier {supplier.Name} found",
                    Data = _mapper.Map<SupplierDTO>(supplier),
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

        public async Task<ApplicationResponse> GetSuppliersAsync(string? supplierName, int? supplierCategoryId, string? province, bool? isFeatured, decimal? averageRating, decimal? minPrice, decimal? maxPrice, bool? sortAsc, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var suppliersPagedList = await _supplierRepository.GetSuppliersAsync(supplierName, supplierCategoryId, province, isFeatured, averageRating, minPrice, maxPrice, sortAsc, pageNumber, pageSize);

                var suppliersDTO = _mapper.Map<List<SupplierDTO>>(suppliersPagedList);

                var responseData = new PagedData<SupplierDTO>
                {
                    Items = suppliersDTO,
                    PageNumber = suppliersPagedList.PageNumber,
                    PageSize = suppliersPagedList.PageSize,
                    TotalItemCount = suppliersPagedList.TotalItemCount,
                    PageCount = suppliersPagedList.PageCount,
                    IsFirstPage = suppliersPagedList.IsFirstPage,
                    IsLastPage = suppliersPagedList.IsLastPage,
                    HasNextPage = suppliersPagedList.HasNextPage,
                    HasPreviousPage = suppliersPagedList.HasPreviousPage
                };


                return new ApplicationResponse
                {
                    Message = "Suppliers retrieved successfully",
                    Data = responseData,
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
      
        public async Task<ApplicationResponse> UpdateSupplierProfileAsync(UpdateSupplierProfileDTO supplierProfileDTO)
        {
            try
            {
                var currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                var supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);

                if (supplier == null) throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                var category = await _supplierCategoryRepository.GetSupplierCategoryByIdAsync(supplierProfileDTO.CategoryId);

                if (category == null) throw new ApiException("Category not found", System.Net.HttpStatusCode.NotFound);

                if (supplierProfileDTO.Price > Constants.Common.MAX_PRICE) throw new ApiException("Price is too high", System.Net.HttpStatusCode.BadRequest);

                var updatedSupplier = _mapper.Map(supplierProfileDTO, supplier);

                if (supplier.OnBoardStatus != OnBoardStatus.COMPLETED)
                {
                    updatedSupplier.OnBoardStatus = OnBoardStatus.SUPPLIER_LOCATION; // Chuyển trạng thái của supplier sang bước cập nhật địa chỉ
                }

                var result = await _supplierRepository.UpdateSupplierAsync(updatedSupplier);

                if (result == null) throw new ApiException("Failed to update supplier", System.Net.HttpStatusCode.InternalServerError);

                return new ApplicationResponse
                {
                    Message = "Supplier profile updated successfully",
                    Data = _mapper.Map<SupplierDTO>(result),
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

        public async Task<ApplicationResponse> UpdateSupplierAddressAsync(UpdateSupplierAddressDTO supplierAddressDTO)
        {
            try
            {
                var currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                var supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);

                if (supplier == null) throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                var updatedSupplier = _mapper.Map(supplierAddressDTO, supplier);

                if (supplier.OnBoardStatus != OnBoardStatus.COMPLETED)
                {
                    updatedSupplier.OnBoardStatus = OnBoardStatus.SUPPLIER_IMAGES; // Chuyển trạng thái của supplier sang bước tải lên hình ảnh
                }

                var result = await _supplierRepository.UpdateSupplierAsync(updatedSupplier);

                if (result == null) throw new ApiException("Failed to update supplier", System.Net.HttpStatusCode.InternalServerError);

                return new ApplicationResponse
                {
                    Message = "Supplier address updated successfully",
                    Data = _mapper.Map<SupplierDTO>(result),
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

        public async Task<ApplicationResponse> UploadSupplierImages(List<IFormFile> files, int? primaryImageIndex)
        {
            try
            {
                if (files == null || !files.Any())
                {
                    throw new ApiException("No files were uploaded.", System.Net.HttpStatusCode.BadRequest);
                }

                if ((primaryImageIndex < 0 || primaryImageIndex >= files.Count()) && primaryImageIndex != null)
                {
                    throw new ApiException("Invalid primary image index.", System.Net.HttpStatusCode.BadRequest);
                }

                var currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                var supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);
                if (supplier == null)
                {
                    throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);
                }

                var uploadedImages = new List<SupplierImage>();

                // Upload each file to Azure Blob Storage
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    var uploadResult = await _storageService.UploadAsync(file);

                    if (uploadResult.Error)
                    {
                        throw new ApiException($"Failed to upload file {file.FileName}.", System.Net.HttpStatusCode.InternalServerError);
                    }

                    uploadedImages.Add(new SupplierImage
                    {
                        ImageUrl = uploadResult.Blob.Uri,
                        ImageFileName = uploadResult.Blob.Name,
                        SupplierId = supplier.Id,
                        IsPrimary = (i == primaryImageIndex)
                    });
                }

                 await _supplierImageRepository.AddSupplierImagesAsync(uploadedImages);

                if (supplier.OnBoardStatus != OnBoardStatus.COMPLETED)
                {
                    supplier.OnBoardStatus = OnBoardStatus.COMPLETED;
                    var supplierUpdated = await _supplierRepository.UpdateSupplierAsync(supplier);
                    
                }

                return new ApplicationResponse
                {
                    Message = "Images uploaded successfully.",
                    Data = _mapper.Map<List<SupplierImageDTO>>(uploadedImages),
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

        public async Task<ApplicationResponse> DeleteSupplierImageAsync(int imageId)
        {
            try
            {
                var supplierImage = await _supplierImageRepository.GetSupplierImageByIdAsync(imageId);

                if (supplierImage == null)  throw new ApiException("Image not found", System.Net.HttpStatusCode.NotFound);

                var supplierImages = await _supplierImageRepository.GetSupplierImagesBySupplierIdAsync(supplierImage.SupplierId);

                if (supplierImages.Count() == 1) throw new ApiException("Cannot delete the only image", System.Net.HttpStatusCode.BadRequest);

                await _supplierImageRepository.DeleteSupplierImageAsync(imageId);

                var result = await _storageService.DeleteAsync(supplierImage.ImageFileName!);

                if (result.Error) throw new ApiException(result.Status!, System.Net.HttpStatusCode.InternalServerError);
                
                return new ApplicationResponse
                {
                    Message = "Supplier image deleted successfully",
                    Data = _mapper.Map<SupplierImageDTO>(supplierImage),
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

        public async Task<ApplicationResponse> UpdatePrimaryImageAsync(int imageId)
        {
            try
            {
                var supplierImage = await _supplierImageRepository.GetSupplierImageByIdAsync(imageId);

                if (supplierImage == null)  throw new ApiException("Image not found", System.Net.HttpStatusCode.NotFound);

                var primaryImage = await _supplierImageRepository.GetPrimaryImageBySupplierId(supplierImage.SupplierId);

                if (primaryImage != null)
                {
                    primaryImage.IsPrimary = false;
                    await _supplierImageRepository.UpdatePrimaryImageAsync(primaryImage);
                }

                supplierImage.IsPrimary = true;

                await _supplierImageRepository.UpdatePrimaryImageAsync(supplierImage);

                return new ApplicationResponse
                {
                    Message = "Supplier image updated successfully",
                    Data = _mapper.Map<SupplierImageDTO>(supplierImage),
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

        public async Task<ApplicationResponse> GetSupplierImagesBySupplier(int supplierId)
        {
            try
            {
                var supplier = await _supplierRepository.GetSupplierByIdAsync(supplierId, false);

                if (supplier == null) throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                var supplierImages = await _supplierImageRepository.GetSupplierImagesBySupplierIdAsync(supplierId);

                return new ApplicationResponse
                {
                    Message = "Get images successfully",
                    Data = _mapper.Map<List<SupplierImageDTO>>(supplierImages),
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
