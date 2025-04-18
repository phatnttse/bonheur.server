﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class OrderDetail: BaseEntity
    {
        public int OrderId { get; set; }

        [JsonIgnore]   
        public virtual Order? Order { get; set; }

        public int? SubscriptionPackageId { get; set; } 
        public virtual SubscriptionPackage? SubscriptionPackage { get; set; }

        public int? AdvertisementId { get; set; } 
        public virtual Advertisement? Advertisement { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public decimal Price { get; set; } 

        public int Quantity { get; set; } 

        [Required]
        public decimal TotalAmount { get; set; } 
    }
}
