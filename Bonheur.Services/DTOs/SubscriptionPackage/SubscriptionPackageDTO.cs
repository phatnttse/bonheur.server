using System.ComponentModel.DataAnnotations;


namespace Bonheur.Services.DTOs.SubscriptionPackage
{
    public class SubscriptionPackageDTO
    {
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

        [Required]
        public int Priority { get; set; }
    }
}
