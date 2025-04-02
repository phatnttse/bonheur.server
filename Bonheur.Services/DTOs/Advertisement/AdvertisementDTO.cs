using Bonheur.Services.DTOs.AdPackage;
using Bonheur.Services.DTOs.Supplier;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Advertisement
{
    public class AdvertisementDTO
    {
        public int Id { get; set; }
        public SupplierDTO? Supplier { get; set; }
        public AdPackageDTO? AdPackage { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageFileName { get; set; }
        public string? VideoUrl { get; set; } 
        public string? VideoFileName { get; set; } 
        public string? TargetUrl { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
