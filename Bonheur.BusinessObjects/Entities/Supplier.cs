using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Bonheur.BusinessObjects.Entities
{
    public class Supplier : BaseEntity
    {

        [ForeignKey("UserId")]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("SupplierCategoryId")]
        public int SupplierCategoryId { get; set; }
        public virtual SupplierCategory? SupplierCategory { get; set; }

        [Required]
        [StringLength(255)]
        public string? SupplierName { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? SupplierDescription { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        public string? Street { get; set; }
        public string? Province { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }

        [Required]
        [StringLength(15)]
        public string? ContactPhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string? ContactEmail { get; set; }

        [Url]
        [StringLength(255)]
        public string? WebsiteUrl { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? ResponseTime { get; set; }

        // Trường để xác định thứ tự ưu tiên
        public int PriorityScore { get; set; } = 0; 

        public bool IsFeatured { get; set; } = false; // Mặc định không được hiển thị nổi bật

        public DateTime? BoostUntil { get; set; } // Thời gian ưu tiên

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AverageRating { get; set; } = 0;

        // Gói dịch vụ
        public int? SubscriptionPackageId { get; set; }
        public virtual SubscriptionPackage? SubscriptionPackage { get; set; }

        // Quảng cáo
        public virtual ICollection<Advertisement>? Advertisements { get; set; }

        // Hình ảnh
        public virtual ICollection<SupplierImage>? SupplierImages { get; set; }


    }
}
