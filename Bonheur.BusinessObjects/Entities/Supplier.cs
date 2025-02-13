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

        public string? Address 
        { 
            get
            {
                return $"{Street}, {Ward}, {District}, {Province}";
            }
        }

        [Url]
        [StringLength(255)]
        public string? WebsiteUrl { get; set; }

        public string? ResponseTime { get; set; }

        public int Priority { get; set; } = 0; 

        public bool IsFeatured { get; set; } = false; 

        public DateTimeOffset? PriorityEnd { get; set; }

        [EnumDataType(typeof(SupplierStatus))]
        public SupplierStatus? Status { get; set; } = SupplierStatus.Pending;

        [StringLength(50)]
        [EnumDataType(typeof(OnBoardStatus))]
        public OnBoardStatus? OnBoardStatus { get; set; } = Enums.OnBoardStatus.BusinessInfo;
        public int OnBoardPercent
        {
            get
            {
                switch (OnBoardStatus)
                {
                    case Enums.OnBoardStatus.BusinessInfo:
                        return 25;
                    case Enums.OnBoardStatus.Location:
                        return 50;
                    case Enums.OnBoardStatus.Photos:
                        return 75;
                    case Enums.OnBoardStatus.Completed:
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

        // Danh sách mạng xã hội của Supplier
        public virtual ICollection<SupplierSocialNetwork>? SocialNetworks { get; set; }

        // Danh sách FAQ của Supplier
        public virtual ICollection<SupplierFAQ>? Faqs { get; set; }

    }
}
