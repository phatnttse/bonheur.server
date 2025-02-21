using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Supplier
{
    public class UpdateSupplierAddressDTO
    {
        public string? Street { get; set; }

        [Required]
        public required string Province { get; set; }

        [Required]
        public required string Ward { get; set; }

        [Required]
        public required string District { get; set; }

        [Required]
        public required string Longitude { get; set; }

        [Required]
        public required string Latitude { get; set; }
    }
}
