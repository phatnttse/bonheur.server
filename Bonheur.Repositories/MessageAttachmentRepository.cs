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
    public class MessageAttachmentRepository : IMessageAttachmentRepository
    {
        private readonly MessageAttachmentDAO _messageAttachmentDAO;

        public MessageAttachmentRepository(MessageAttachmentDAO messageAttachmentDAO)
        {
            _messageAttachmentDAO = messageAttachmentDAO;
        }

        public async Task<MessageAttachment> CreateMessageAttachmentAsync(MessageAttachment messageAttachment) => await _messageAttachmentDAO.CreateMessageAttachmentAsync(messageAttachment);

        public async Task<MessageAttachment?> GetMessageAttachmentByIdAsync(int id) => await _messageAttachmentDAO.GetMessageAttachmentByIdAsync(id);

        public async Task<MessageAttachment> DeleteMessageAttachmentAsync(MessageAttachment messageAttachment) => await _messageAttachmentDAO.DeleteMessageAttachmentAsync(messageAttachment);

        public async Task<MessageAttachment> UpdateMessageAttachmentAsync(MessageAttachment messageAttachment) => await _messageAttachmentDAO.UpdateMessageAttachmentAsync(messageAttachment);
    }
}
