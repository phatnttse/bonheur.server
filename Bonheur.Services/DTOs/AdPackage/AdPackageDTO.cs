using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.AdPackage
{
    public class AdPackageDTO
    {
        private int Id { get; set; }
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public DateTimeOffset StartDate { get; set; }

        [Required]
        public DateTimeOffset EndDate { get; set; }

        [Required]
        [EnumDataType(typeof(AdType))]
        public AdType AdType { get; set; }

        [Required]
        public bool IsActive { get; set; } = true; 
    }
}
