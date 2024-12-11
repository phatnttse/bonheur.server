using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class SubscriptionPackage : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string? Name { get; set; } // Tên gói (ví dụ: Gói Cơ Bản, Gói Cao Cấp)

        [Required]
        public string? Description { get; set; } // Mô tả ngắn về gói (ví dụ: "Ưu tiên hiển thị lên trang đầu")

        [Required]
        public int DurationInDays { get; set; } // Số ngày sử dụng gói

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // Giá gói

        public bool IsFeatured { get; set; } // Có hiển thị trên trang đầu không?

        public int Priority { get; set; } // Mức ưu tiên khi tìm kiếm (số càng cao, càng ưu tiên) (ex: 0, 1, 2, 3)

        public bool IsDeleted { get; set; } = false; 
    }
}
