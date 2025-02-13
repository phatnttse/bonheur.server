using System.ComponentModel.DataAnnotations;


namespace Bonheur.Services.DTOs.SubscriptionPackage
{
    public class SubscriptionPackageDTO
    {
        public int? Id { get; set; }

        [Required]
        public string? Name { get; set; } 

        [Required]
        public string? Description { get; set; } 

        [Required]
        public int DurationInDays { get; set; } 

        [Required]
        public decimal Price { get; set; }

        [Required]
        public bool IsFeatured { get; set; }

        public string? BadgeText { get; set; }

        public string? BadgeColor { get; set; }

        public string? BadgeTextColor { get; set; }

        [Required]
        public int Priority { get; set; }
    }

}
