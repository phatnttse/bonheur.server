using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Supplier
{
    public class CreateSupplierDTO
    {
        [Required]
        public string? SupplierName { get; set; }

        [Required]
        public int SupplierCategoryId { get; set; }

        [Required]
        public string? PhoneNumber { get; set; }

        public string? WebsiteUrl { get; set; }

    }
}
