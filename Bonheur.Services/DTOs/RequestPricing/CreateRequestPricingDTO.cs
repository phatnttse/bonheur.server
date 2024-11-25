using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.RequestPricing
{
    public class CreateRequestPricingDTO
    {
        public string Name { get; set; } 
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime EventDate { get; set; }
        public string? Message { get; set; } 
        public int SupplierId { get; set; }
        public DateTime ExpirationDate { get; set; } 
    }
}
