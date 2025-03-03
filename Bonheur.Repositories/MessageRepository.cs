using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageDAO _messageDAO;
        public MessageRepository(MessageDAO messageDAO)
        {
            _messageDAO = messageDAO;
        }

        public async Task AddMessage(Message message) => await _messageDAO.Add(message);
        public object GetSupplierMessageStatistics(string userId) => _messageDAO.GetSupplierMessageStatistics(userId);
        public async Task<List<Message>> GetMessages(string userId, string recipientId, int pageNumber, int pageSize) => await _messageDAO.GetMessages(userId, recipientId, pageNumber, pageSize);

        public async Task<Message?> GetMessageByIdAsync(int id) => await _messageDAO.GetMessageByIdAsync(id);
        public async Task UpdateMessage(Message message) => await _messageDAO.UpdateMessage(message);

        public Task<List<string?>> GetSupplierIdsContacted(string userId) => _messageDAO.GetSupplierIdsContacted(userId);
        public Task<Dictionary<string, int>> GetUnredMessagesBySupplierIds(string userId, List<string> supplierIds) => _messageDAO.GetUnredMessagesBySupplierIds(userId, supplierIds);
        public async Task<int> GetUnreadMessagesCountByUserId(string userId) => await _messageDAO.GetUnreadMessagesCountByUserId(userId);
        public Task<Dictionary<string, (string LatestMessage, DateTimeOffset? LatestMessageAt)>> GetLatestMessageBySupplierIds(string userId, List<string> supplierIds) => _messageDAO.GetLatestMessageBySupplierIds(userId, supplierIds);
    }
}
