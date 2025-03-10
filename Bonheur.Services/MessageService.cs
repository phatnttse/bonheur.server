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

        public async Task<ApplicationResponse> UploadAttachmentFile(List<IFormFile> files)
        { 
            try
            {
                if (files.Count == 0) throw new ApiException("Please select a file to upload!", System.Net.HttpStatusCode.BadRequest);

                if (files.Count > 5) throw new ApiException("You can only upload up to 5 files at a time!", System.Net.HttpStatusCode.BadRequest);

                List<AzureBlobResponseDTO> uploadResponses = new List<AzureBlobResponseDTO>();

                foreach (IFormFile file in files)
                {
                    if (file.Length == 0) throw new ApiException("File is empty!", System.Net.HttpStatusCode.BadRequest);

                    if (file.Length > 10485760) throw new ApiException("File size is too large! Maximum file size is 10MB.", System.Net.HttpStatusCode.BadRequest);

                    AzureBlobResponseDTO uploadResult = await _storageService.UploadAsync(file);

                    uploadResponses.Add(uploadResult);

                }

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Upload attachment file successfully!",
                    Data = uploadResponses,
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
