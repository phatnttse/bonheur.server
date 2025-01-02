using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class Advertisement : BaseEntity
    {

        [Required]
        public int SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }

        [Required]
        public int AdPackageId { get; set; } // Mối quan hệ với gói quảng cáo đã chọn
        public virtual AdPackage? AdPackage { get; set; } // Quảng cáo sử dụng gói quảng cáo nào

        public string? Title { get; set; } // Tiêu đề quảng cáo

        public string? Content { get; set; } // Nội dung quảng cáo

        public string? ImageUrl { get; set; } // URL hình ảnh quảng cáo

        public string? TargetUrl { get; set; } // URL khi người dùng click vào quảng cáo

        public bool IsActive { get; set; } = true; // Trạng thái của quảng cáo
    }
}
