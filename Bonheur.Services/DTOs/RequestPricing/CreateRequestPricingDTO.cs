﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.RequestPricing
{
    public class CreateRequestPricingDTO
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? PhoneNumber { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string? Message { get; set; }

        [Required]
        public int SupplierId { get; set; }

    }
}
