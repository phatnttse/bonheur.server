using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        public MessageService(IMessageRepository messageRepository, IUserAccountRepository userAccountRepository)
        {
            _messageRepository = messageRepository;
            _userAccountRepository = userAccountRepository;
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
    }
}
