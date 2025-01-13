using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class AdPackage : BaseEntity
    {
        [Required]
        public string? Title { get; set; } 

        public string? Description { get; set; } 

        public decimal Price { get; set; } 

        public DateTimeOffset StartDate { get; set; } 

        public DateTimeOffset EndDate { get; set; }

        [EnumDataType(typeof(AdType))]
        public AdType AdType { get; set; } 

        public bool IsActive { get; set; } = true; 

    }
}
