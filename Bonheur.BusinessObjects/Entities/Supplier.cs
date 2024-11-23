using Bonheur.BusinessObjects.Enums;
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

        [StringLength(100)]
        public string? Street { get; set; }

        [StringLength(100)]
        public string? Province { get; set; }

        [StringLength(100)]
        public string? Ward { get; set; }

        [StringLength(100)]
        public string? District { get; set; }

        [Url]
        [StringLength(255)]
        public string? WebsiteUrl { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? ResponseTime { get; set; }

        public int Priority { get; set; } = 0; 

        public bool IsFeatured { get; set; } = false; 

        public DateTime? ProrityEnd { get; set; }

        [StringLength(50)]
        public SupplierStatus? Status { get; set; } = SupplierStatus.PENDING;

        [StringLength(50)]
        public OnBoardStatus? OnBoardStatus { get; set; } = Enums.OnBoardStatus.SUPPLIER_INFO;
        public int OnBoardPercent
        {
            get
            {
                switch (OnBoardStatus)
                {
                    case Enums.OnBoardStatus.SUPPLIER_INFO:
                        return 25;
                    case Enums.OnBoardStatus.SUPPLIER_LOCATION:
                        return 50;
                    case Enums.OnBoardStatus.SUPPLIER_IMAGES:
                        return 75;
                    case Enums.OnBoardStatus.COMPLETED:
                        return 100;
                    default:
                        return 0;
                }
            }
        }

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
