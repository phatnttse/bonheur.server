﻿using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Advertisement;
using Bonheur.Services.DTOs.Storage;
using Bonheur.Services.Interfaces;
using Bonheur.Services.MessageBrokers.Events;
using Bonheur.Utils;

namespace Bonheur.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IMapper _mapper;
        private readonly IAdPackageRepository _adPackageRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IStorageService _storageService;
        private readonly IEventBus _eventBus;
        private readonly INotificationRepository _notificationRepository;

        public AdvertisementService(
            IAdvertisementRepository advertisementRepository, 
            IMapper mapper,
            IAdPackageRepository adPackageRepository,
            ISupplierRepository supplierRepository,
            IStorageService storageService,
            IEventBus eventBus,
            INotificationRepository notificationRepository
        )
        {
            _advertisementRepository = advertisementRepository;
            _mapper = mapper;
            _adPackageRepository = adPackageRepository;
            _supplierRepository = supplierRepository;
            _storageService = storageService;
            _eventBus = eventBus;
            _notificationRepository = notificationRepository;
        }

        public async Task<ApplicationResponse> AddAdvertisementAsync(CreateAdvertisementDTO advertisementDTO)
        {
            try
            {

                Supplier supplier = await _supplierRepository.GetSupplierByIdAsync(advertisementDTO.SupplierId, false) ??
                    throw new ApiException("Supplier does not exist!");

                AdPackage adPackage = await _adPackageRepository.GetAdPackageById(advertisementDTO.AdPackageId) ??
                    throw new ApiException("Ad Package does not exist!");

                var advertisement = _mapper.Map<Advertisement>(advertisementDTO);
                advertisement.SupplierId = supplier.Id;
                advertisement.AdPackageId = adPackage.Id;
                advertisement.Status = AdvertisementStatus.Pending;
                advertisement.PaymentStatus = PaymentStatus.Pending;

                if (advertisementDTO.Image != null)
                {
                    AzureBlobResponseDTO response = await _storageService.UploadAsync(advertisementDTO.Image);
                    advertisement.ImageUrl = response.Blob.Uri;
                    advertisement.ImageFileName = response.Blob.Name;
                }

                if (advertisementDTO.Video != null)
                {
                    AzureBlobResponseDTO response = await _storageService.UploadAsync(advertisementDTO.Video);
                    advertisement.VideoUrl = response.Blob.Uri;
                    advertisement.VideoFileName = response.Blob.Name;
                }

                await _advertisementRepository.AddAdvertisementAsync(advertisement);

                NotificationCreatedEvent createdEvent = new NotificationCreatedEvent
                {
                    Id = advertisement.Id,
                    Title = "New Advertisement",
                    Content = $"New advertisement from {supplier.Name} has been created",
                    RecipientId = supplier.UserId,
                    Type = NotificationType.SystemAlert,
                    Link = $"{Constants.Common.CLIENT_URL}/admin/advertisement/management",
                    IsRead = false,
                    CreatedAt = advertisement.CreatedAt,
                    UpdatedAt = advertisement.UpdatedAt,
                };

                await _eventBus.PublishAsync(createdEvent);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Advertisement added successfully!",
                    Data = advertisement,
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
                    Message = "Advertisement deleted successfully!",
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
                var advertisement = await _advertisementRepository.GetAdvertisementByIdAsync(id) ?? throw new ApiException("Advertisement not found!");
                
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

        public async Task<ApplicationResponse> GetAdvertisementBySupplierAsync(int pageNumber, int pageSize)
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);

                Supplier? supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId);

                if (supplier == null) throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                var listAdvertisement = await _advertisementRepository.GetAdvertisementsBySupplier(supplier.Id, pageNumber, pageSize);

                var listAdvertisementDTO = _mapper.Map<List<AdvertisementDTO>>(listAdvertisement);

                var responseData = new PagedData<AdvertisementDTO>
                {
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

        public async Task<ApplicationResponse> GetAdvertisementsAsync(string? searchTitle, string? searchContent, int pageNumber = 1, int pageSize = 10)
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

        public async Task<ApplicationResponse> UpdateAdvertisementAsync(int id, UpdateAdvertisementDTO advertisementDTO)
        {
            try
            {
                Advertisement existedAdvertisement = await _advertisementRepository.GetAdvertisementByIdAsync(id) ??
                    throw new ApiException("Advertisement not found!");

                var previousStatus = existedAdvertisement.Status;

                _mapper.Map(existedAdvertisement, advertisementDTO);

                if (advertisementDTO.Image != null)
                {
                    AzureBlobResponseDTO response = await _storageService.UploadAsync(advertisementDTO.Image);
                    existedAdvertisement.ImageUrl = response.Blob.Uri;
                    existedAdvertisement.ImageFileName = response.Blob.Name;
                }

                if (advertisementDTO.Video != null)
                {
                    AzureBlobResponseDTO response = await _storageService.UploadAsync(advertisementDTO.Video);
                    existedAdvertisement.VideoUrl = response.Blob.Uri;
                    existedAdvertisement.VideoFileName = response.Blob.Name;
                }

                await _advertisementRepository.UpdateAdvertisementAsync(existedAdvertisement);

                if (previousStatus != existedAdvertisement.Status)
                {
                    Notification notification = new Notification
                    {
                        Title = "Advertisement Status Update",
                        Content = $"Your advertisement with title {existedAdvertisement.Title} has been updated to {existedAdvertisement.Status}",
                        RecipientId = existedAdvertisement.Supplier.UserId,
                        Type = NotificationType.SystemAlert,
                        Link = $"{Constants.Common.CLIENT_URL}/supplier/advertisement",
                    };

                    await _notificationRepository.CreateNotificationAsync(notification);

                    NotificationCreatedEvent notificationEvent = new NotificationCreatedEvent
                    {
                        Id = notification.Id,
                        Title = "Advertisement Status Update",
                        Content = $"Your advertisement with title {existedAdvertisement.Title} has been updated to {existedAdvertisement.Status}",
                        RecipientId = existedAdvertisement.Supplier.UserId,
                        Type = NotificationType.SystemAlert,
                        Link = $"{Constants.Common.CLIENT_URL}/supplier/advertisement",
                        IsRead = false,
                        CreatedAt = notification.CreatedAt,
                        UpdatedAt = notification.UpdatedAt,
                    };

                    await this._eventBus.PublishAsync(notificationEvent);

                }

                var updatedAdvertisement = _mapper.Map<AdvertisementDTO>(existedAdvertisement);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Advertisement updated successfully!",
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
