﻿using System;
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
        public decimal Price { get; set; } // Giá gói

        public bool IsFeatured { get; set; } // Có hiển thị trên trang đầu không?

        public int Priority { get; set; } // Mức ưu tiên khi tìm kiếm (số càng cao, càng ưu tiên) (ex: 0, 1, 2, 3)

        public string? BadgeText { get; set; } // Ví dụ: "Ưu tiên", "Hot", "Mới"

        public string? BadgeColor { get; set; } // Màu của badge (ví dụ: "red", "green", "blue")

        public string? BadgeTextColor { get; set; } // Màu chữ của badge (ví dụ: "white", "black")

        public bool IsDeleted { get; set; } = false; 
    }
}
