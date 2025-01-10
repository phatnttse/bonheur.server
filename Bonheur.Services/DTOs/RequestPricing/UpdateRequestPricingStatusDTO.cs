using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.RequestPricing
{
    public class UpdateRequestPricingStatusDTO
    {
        [Required]
        public RequestPricingStatus Status { get; set; }
    }
}
