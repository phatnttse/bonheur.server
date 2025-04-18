﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Supplier
{
    public class UpdateSupplierProfileDTO
    {
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? WebsiteUrl { get; set; }

        [Required]
        public required decimal Price { get; set; }

        [Required]
        public required string Description { get; set; }

        public TimeSpan ResponseTimeStart { get; set; }

        public TimeSpan ResponseTimeEnd { get; set; }

        public decimal Discount { get; set; }

    }
}
