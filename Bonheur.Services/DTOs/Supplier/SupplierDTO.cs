using Bonheur.BusinessObjects.Enums;
using Bonheur.Services.DTOs.Review;
using Bonheur.Services.DTOs.SocialNetwork;
using Bonheur.Services.DTOs.SubscriptionPackage;
using Bonheur.Services.DTOs.SupplierCategory;
using Bonheur.Services.DTOs.SupplierFAQ;

namespace Bonheur.Services.DTOs.Supplier
{
    public class SupplierDTO
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public SupplierCategoryDTO? Category { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Street { get; set; }
        public string? Province { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string Address
        {
            get
            {
                return $"{Street}, {Ward}, {District}, {Province}".Trim(',', ' ');
            }
        }
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
        public string? WebsiteUrl { get; set; }
        public TimeSpan ResponseTimeStart { get; set; }
        public TimeSpan ResponseTimeEnd { get; set; }
        public int Priority { get; set; }
        public DateTimeOffset? PriorityEnd { get; set; }
        public SupplierStatus? Status { get; set; } 
        public OnBoardStatus? OnBoardStatus { get; set; }
        public bool IsFeatured { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRating { get; set; } 
        public decimal Discount { get; set; }
        public int View { get; set; }
        public int StepCompletedCount { get; set; }
        public bool IsStep1Completed { get; set; }
        public bool IsStep2Completed { get; set; }
        public bool IsStep3Completed { get; set; }
        public List<SupplierImageDTO>? Images { get; set; }
        public List<SupplierVideoDTO>? Videos { get; set; }
        public List<SupplierSocialNetworkDTO>? SocialNetworks { get; set; }
        public List<SupplierFAQDTO>? Faqs { get; set; }
        public SubscriptionPackageDTO? SubscriptionPackage { get; set; }
        public List<ReviewDTO>? Reviews { get; set; }

    }
}
