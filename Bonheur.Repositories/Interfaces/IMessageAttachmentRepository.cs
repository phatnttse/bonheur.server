using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface IMessageAttachmentRepository
    {
        Task<MessageAttachment> CreateMessageAttachmentAsync(MessageAttachment messageAttachment);
        Task<MessageAttachment?> GetMessageAttachmentByIdAsync(int id);
        Task<MessageAttachment> DeleteMessageAttachmentAsync(MessageAttachment messageAttachment);

        Task<MessageAttachment> UpdateMessageAttachmentAsync(MessageAttachment messageAttachment);
    }
}
