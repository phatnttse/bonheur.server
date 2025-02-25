using Bonheur.BusinessObjects.Entities;
using Bonheur.Utils;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using PdfSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.DAOs
{
    public class MessageDAO
    {
        private readonly ApplicationDbContext _context;
        public MessageDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public object GetSupplierMessageStatistics(string userId)
        {

                var totalMessages = _context.Messages
                    .Where(m => (m.ReceiverId == userId && m.ReceiverRole == Constants.Roles.SUPPLIER))
                    .Count();

                var unreadMessages = _context.Messages
                    .Where(m => m.ReceiverId == userId && m.ReceiverRole == Constants.Roles.SUPPLIER && !m.IsRead)
                    .Count();

            return new
            {
                TotalMessages = totalMessages,
                UnreadMessages = unreadMessages
            };                
        }
        public async Task<List<Message>> GetMessages(string userId, string recipientId, int pageNumber, int pageSize)
        {
           return  await _context.Messages
            .Where(m => (m.SenderId == userId && m.ReceiverId == recipientId) ||
                (m.SenderId == recipientId && m.ReceiverId == userId))
                .OrderBy(m => m.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        }

        public async Task<Message?> GetMessageByIdAsync(int id)
        {
            return await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task UpdateMessage(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }

        // Lấy danh sách ID của các supplier mà userId đã từng gửi tin nhắn
        public Task<List<string?>> GetSupplierIdsContacted(string userId)
        {
            var supplierIds =  _context.Messages
                .Where(m => m.SenderId == userId && m.ReceiverRole == Constants.Roles.SUPPLIER && m.SenderRole == Constants.Roles.USER)
                .Select(m => m.ReceiverId)
                .Distinct()
                .ToList();

            return Task.FromResult(supplierIds);
        }

        // Lấy số lượng tin nhắn chưa đọc theo từng supplier
        public Task<Dictionary<string, int>> GetUnredMessagesBySupplierIds(string userId, List<string> supplierIds)
        {
            var unreadMessagesDict = _context.Messages
                .Where(m => m.ReceiverId == userId && !m.IsRead && supplierIds.Contains(m.SenderId!))
                .GroupBy(m => m.SenderId)
                .Select(g => new { SupplierId = g.Key, UnreadCount = g.Count() })
                .ToDictionary(g => g.SupplierId!, g => g.UnreadCount);

            return Task.FromResult(unreadMessagesDict); ;
        }

        public async Task<int> GetUnreadMessagesCountByUserId(string userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId && !m.IsRead && m.ReceiverRole == Constants.Roles.USER)
                .CountAsync();
        }

    }
}
