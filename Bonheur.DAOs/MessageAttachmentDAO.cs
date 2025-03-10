using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.DAOs
{
    public class MessageAttachmentDAO
    {
        private readonly ApplicationDbContext _context;

        public MessageAttachmentDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MessageAttachment> CreateMessageAttachmentAsync(MessageAttachment messageAttachment)
        {
            await _context.MessageAttachments.AddAsync(messageAttachment);
            await _context.SaveChangesAsync();
            return messageAttachment;
        }

        public async Task<MessageAttachment?> GetMessageAttachmentByIdAsync(int id) => await _context.MessageAttachments.FindAsync(id);

        public async Task<MessageAttachment> UpdateMessageAttachmentAsync(MessageAttachment messageAttachment)
        {
            _context.MessageAttachments.Update(messageAttachment);
            await _context.SaveChangesAsync();
            return messageAttachment;
        }

        public async Task<MessageAttachment> DeleteMessageAttachmentAsync(MessageAttachment messageAttachment)
        {
            _context.MessageAttachments.Remove(messageAttachment);
            await _context.SaveChangesAsync();
            return messageAttachment;
        }

    }
}
