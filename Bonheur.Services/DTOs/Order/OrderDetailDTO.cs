using Bonheur.Services.DTOs.AdPackage;
using Bonheur.Services.DTOs.SubscriptionPackage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Order
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public SubscriptionPackageDTO? SubscriptionPackage { get; set; }
        public AdPackageDTO? AdPackage { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
