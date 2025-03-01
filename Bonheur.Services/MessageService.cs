using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Message;
using Bonheur.Services.DTOs.Storage;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Http;


namespace Bonheur.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IStorageService _storageService;
        private readonly IMessageAttachmentRepository _messageAttachmentRepository;
        private readonly IMapper _mapper;
        public MessageService(IMessageRepository messageRepository, IUserAccountRepository userAccountRepository, IStorageService storageService, IMessageAttachmentRepository messageAttachmentRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userAccountRepository = userAccountRepository;
            _storageService = storageService;
            _messageAttachmentRepository = messageAttachmentRepository;
            _mapper = mapper;
        }

        public ApplicationResponse GetSupplierMessageStatistics()
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);
                var result = _messageRepository.GetSupplierMessageStatistics(currentUserId);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Get supplier message statistics successfully!",
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

        public async Task<ApplicationResponse> GetUnreadMessagesCountByUser()
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("Please ensure you are logged in.", System.Net.HttpStatusCode.Unauthorized);
                ApplicationUser? user = await _userAccountRepository.GetUserByIdAsync(currentUserId);

                if (user == null) throw new ApiException("User does not exist!", System.Net.HttpStatusCode.NotFound);

                int unreadMessagesCount = await _messageRepository.GetUnreadMessagesCountByUserId(currentUserId);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Get unread messages by user successfully!",
                    Data = unreadMessagesCount,
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

        public async Task<ApplicationResponse> UploadAttachmentFile(IFormFile file)
        {
            try
            {
                AzureBlobResponseDTO uploadResult = await _storageService.UploadAsync(file);

                MessageAttachment attachment = new MessageAttachment
                {
                    FileName = uploadResult.Blob.Name,
                    FilePath = uploadResult.Blob.Uri,
                    FileType = file.ContentType
                };

                MessageAttachment newAttachment = await _messageAttachmentRepository.CreateMessageAttachmentAsync(attachment);

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Upload attachment file successfully!",
                    Data = _mapper.Map<MessageAttachmentDTO>(newAttachment),
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
