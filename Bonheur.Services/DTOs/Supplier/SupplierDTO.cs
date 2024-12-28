using Bonheur.BusinessObjects.Enums;
using Bonheur.Services.DTOs.SupplierCategory;

namespace Bonheur.Services.DTOs.Supplier
{
    public class SupplierDTO
    {
        public int Id { get; set; }
        public SupplierCategoryDTO? SupplierCategory { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierDescription { get; set; }
        public decimal? Price { get; set; }
        public string? Street { get; set; }
        public string? Province { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? ResponseTime { get; set; }
        public int Priority { get; set; }
        public SupplierStatus? SupplierStatus { get; set; } 
        public OnBoardStatus? OnBoardStatus { get; set; }
        public int OnBoardPercent { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime? PriorityEnd { get; set; }
        public decimal AverageRating { get; set; }
        public List<SupplierImageDTO>? SupplierImages { get; set; }
    }
}
