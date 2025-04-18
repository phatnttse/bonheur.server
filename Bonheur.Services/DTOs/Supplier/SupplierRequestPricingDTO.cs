﻿using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.DTOs.SupplierCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Supplier
{
    public class SupplierRequestPricingDTO
    {
        public int Id { get; set; }
        //public UserAccountDTO? User { get; set; }
        public SupplierCategoryDTO? SupplierCategory { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierDescription { get; set; }
        public decimal? Price { get; set; }
        public string? Street { get; set; }
        public string? Province { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? ResponseTime { get; set; }
        public int Priority { get; set; }
        public SupplierStatus? Status { get; set; }
        //public OnBoardStatus? OnBoardStatus { get; set; }
        //public int OnBoardPercent { get; set; }
        //public bool IsFeatured { get; set; }
        //public DateTime? PriorityEnd { get; set; }
        //public decimal AverageRating { get; set; }
        //public SubscriptionPackage? SubscriptionPackage { get; set; }
        //public List<Advertisement>? Advertisements { get; set; }
        //public List<SupplierImageDTO>? SupplierImages { get; set; }
    }
}
