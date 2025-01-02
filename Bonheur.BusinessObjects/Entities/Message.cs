using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class Message : BaseEntity
    {
        public string? SenderId { get; set; }  // Khóa ngoại đến ApplicationUser (người gửi)
        public virtual ApplicationUser? Sender { get; set; } // Người gửi (có thể là Supplier hoặc khách hàng)
        public string? SenderName { get; set; } // Tên người gửi


        public string? ReceiverId { get; set; }  // Khóa ngoại đến ApplicationUser (người nhận)
        public virtual ApplicationUser? Receiver { get; set; } // Người nhận (có thể là Supplier hoặc khách hàng)
        public string? ReceiverName { get; set; } // Tên người nhận
       
        public string? Content { get; set; }  // Nội dung tin nhắn
        public bool IsRead { get; set; }  // Trạng thái đã đọc

        public string? AttachmentUrl { get; set; }  // Đường dẫn tới file đính kèm
        public string? AttachmentType { get; set; }  // Kiểu file đính kèm

        public virtual ICollection<MessageAttachment>? Attachments { get; set; } // Các file đính kèm khác
    }
}
