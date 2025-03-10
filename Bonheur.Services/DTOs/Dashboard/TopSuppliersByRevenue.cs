using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Dashboard
{
    public class TopSuppliersByRevenue
    {
        public int SupplierId { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Street { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? Province { get; set; }
        public string? WebsiteUrl { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRating { get; set; }
        public string? SubscriptionPackage { get; set; }
        public decimal TotalPayment { get; set; }
        public int TotalCompletedOrders { get; set; }
        public string? PrimaryImageUrl { get; set; }
    }
}
