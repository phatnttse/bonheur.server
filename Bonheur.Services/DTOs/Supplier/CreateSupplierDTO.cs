﻿using System;
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
        public string? Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string? PhoneNumber { get; set; }

        public string? WebsiteUrl { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Password { get; set; }

    }
}
