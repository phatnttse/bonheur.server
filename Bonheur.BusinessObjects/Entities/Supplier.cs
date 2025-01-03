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

        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public virtual SupplierCategory? Category { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        [Required]
        public string? Slug { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Description { get; set; }

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

        public string? ResponseTime { get; set; }

        public int Priority { get; set; } = 0; 

        public bool IsFeatured { get; set; } = false; 

        public DateTimeOffset? ProrityEnd { get; set; }

        [EnumDataType(typeof(SupplierStatus))]
        public SupplierStatus? Status { get; set; } = SupplierStatus.PENDING;

        [StringLength(50)]
        [EnumDataType(typeof(OnBoardStatus))]
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

        public decimal Discount { get; set; } = 0;

        public decimal AverageRating { get; set; } = 0;

        // Gói dịch vụ
        public int? SubscriptionPackageId { get; set; }
        public virtual SubscriptionPackage? SubscriptionPackage { get; set; }

        // Quảng cáo
        public virtual ICollection<Advertisement>? Advertisements { get; set; }

        // Hình ảnh
        public virtual ICollection<SupplierImage>? Images { get; set; }


    }
}
