﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Advertisement
{
    public class UpdateAdvertisementDTO
    {
        public int SupplierId { get; set; }
        public int AdPackageId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public string? TargetUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
