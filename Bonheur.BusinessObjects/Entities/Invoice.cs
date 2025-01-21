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
        
        public string? InvoiceNumber { get; set; }

        public string? Description { get; set; }

        public int OrderId { get; set; } 
        public virtual Order? Order { get; set; }

        [Required]
        public decimal TotalAmount { get; set; } 

        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus PaymentStatus { get; set; }

        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public string? FileUrl { get; set; }

        public string? FileName { get; set; }

        public string? TransactionId { get; set; }

        public string? CompanyName { get; set; }

        public string? CompanyAddress { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? UEN { get; set; }

    }
}
