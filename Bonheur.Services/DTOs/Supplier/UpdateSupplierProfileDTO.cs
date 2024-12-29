using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Supplier
{
    public class UpdateSupplierProfileDTO
    {
        public string? SupplierName { get; set; }
        public int? SupplierCategoryId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? WebsiteUrl { get; set; }

        [Required]
        public required decimal Price { get; set; }

        [Required]
        public required string SupplierDescription { get; set; }

        [Required]
        public required string ResponseTime { get; set; }

        public string? Discount { get; set; }

    }
}
