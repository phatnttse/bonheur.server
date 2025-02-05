using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class Invoice : BaseEntity
    {
        [Required]
        public int InvoiceNumber { get; set; }

        public string? Description { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }

        public int OrderId { get; set; } 
        public virtual Order? Order { get; set; }

        [Required]
        public decimal TotalAmount { get; set; } 

        public decimal Discount { get; set; }

        public string? TaxNumber { get; set; }

        public decimal TaxAmount { get; set; }

        public string? FileUrl { get; set; }

        public string? FileName { get; set; }

        public string? TransactionId { get; set; }

        public string? CompanyName { get; set; }

        public string? CompanyAddress { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Website { get; set; }

        public string? UEN { get; set; }

    }
}
