using Bonheur.BusinessObjects.Enums;
using Bonheur.Services.DTOs.Supplier;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.RequestPricing
{
    public class RequestPricingsDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public DateTime? EventDate { get; set; }
        [Required]
        public string? Message { get; set; }
        [Required]
        public RequestPricingStatus? Status { get; set; }
        [Required]
        public int SupplierId { get; set; }
        [Required]
        [ForeignKey("SupplierId")]
        public virtual SupplierRequestPricingDTO? Supplier { get; set; }

        [Required]
        public DateTime? ExpirationDate { get; set; }

        [Required]
        public string? RejectReason { get; set; }
    }
}
