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
        [StringLength(100)]
        public string? Title { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Content { get; set; }

        [Url]
        public string? ImageUrl { get; set; } // URL hình ảnh quảng cáo

        [Url]
        public string? TargetUrl { get; set; } // URL khi click vào quảng cáo

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cost { get; set; } // Chi phí quảng cáo

        public bool IsDeleted { get; set; } = false;
    }
}
