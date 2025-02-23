using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class RequestPricing : BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Message { get; set; }

        [EnumDataType(typeof(RequestPricingStatus))]
        public RequestPricingStatus? Status { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? RejectReason { get; set; }
    }
}
