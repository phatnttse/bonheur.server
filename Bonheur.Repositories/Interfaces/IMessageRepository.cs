using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task AddMessage(Message message);
        object GetSupplierMessageStatistics(string userId);
        Task<List<Message>> GetMessages(string userId, string recipientId, int pageNumber, int pageSize);
        Task<Message?> GetMessageByIdAsync(int id);
        Task UpdateMessage(Message message);
        Task<List<string?>> GetSupplierIdsContacted(string userId);
        Task<Dictionary<string, int>> GetUnredMessagesBySupplierIds(string userId, List<string> supplierIds);
        Task<int> GetUnreadMessagesCountByUserId(string userId);


    }
}
