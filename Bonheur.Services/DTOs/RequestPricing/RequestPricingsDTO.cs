using Bonheur.BusinessObjects.Enums;
using Bonheur.Services.DTOs.Supplier;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.RequestPricing
{
    public class RequestPricingsDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Message { get; set; }
        public RequestPricingStatus? Status { get; set; }
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual SupplierRequestPricingDTO? Supplier { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? RejectReason { get; set; }
    }
}
