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

        public DateTimeOffset StartDate { get; set; } // Ngày bắt đầu

        public DateTimeOffset EndDate { get; set; } // Ngày kết thúc

        [EnumDataType(typeof(AdType))]
        public AdType AdType { get; set; } 

        public bool IsActive { get; set; } = true; 

    }
}
