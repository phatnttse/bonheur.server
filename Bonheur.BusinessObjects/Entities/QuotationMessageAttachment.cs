using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class QuotationMessageAttachment : BaseEntity
    {
        public int QuotationMessageId { get; set; }
        public virtual QuotationMessage? QuotationMessage { get; set; } // Tin nhắn

        public string? FileName { get; set; } // Tên file đính kèm
        public string? FilePath { get; set; } // Đường dẫn đến file lưu trữ
        public string? FileType { get; set; } // Kiểu file (image, pdf, etc.)
    }

}
