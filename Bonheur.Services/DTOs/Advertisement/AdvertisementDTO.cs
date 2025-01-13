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
        [Required]
        public int SupplierId { get; set; }
        public virtual SupplierDTO? Supplier { get; set; }

        [Required]
        public int AdPackageId { get; set; }
        public virtual AdPackageDTO? AdPackage { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? ImageUrl { get; set; }

        public string? TargetUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
