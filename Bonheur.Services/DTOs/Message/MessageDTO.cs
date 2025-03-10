using Bonheur.BusinessObjects.Entities;
using Bonheur.Services.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Message
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string? SenderId { get; set; }  
        public virtual UserAccountDTO? Sender { get; set; }
        public string? SenderName { get; set; }
        public string? SenderRole { get; set; }
        public string? ReceiverId { get; set; }  
        public virtual UserAccountDTO? Receiver { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverRole { get; set; } 
        public string? Content { get; set; }  
        public bool IsRead { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public List<MessageAttachmentDTO>? Attachments { get; set; }
    }
}
