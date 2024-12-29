using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class AdPackage : BaseEntity
    {
        [Required]
        public string? Title { get; set; } // Tên gói quảng cáo

        public string? Description { get; set; } // Mô tả gói quảng cáo

        public decimal Price { get; set; } // Giá của gói quảng cáo

        public DateTime StartDate { get; set; } // Ngày bắt đầu

        public DateTime EndDate { get; set; } // Ngày kết thúc

        public AdType AdType { get; set; } // Loại quảng cáo (ví dụ: banner, popup)

        public bool IsActive { get; set; } = true; // Trạng thái gói quảng cáo

    }
}
