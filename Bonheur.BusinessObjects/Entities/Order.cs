using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class Order : BaseEntity
    {
        [Required]
        public int OrderCode { get; set; } 

        public string? UserId { get; set; } 
        public virtual ApplicationUser? User { get; set; } 

        public int? SupplierId {  get; set; }
        public virtual Supplier? Supplier { get; set; }

        [Required]
        public decimal TotalAmount { get; set; } 

        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; }

        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus PaymentStatus { get; set; }

        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }

        public int? InvoiceId { get; set; } 
        public virtual Invoice? Invoice { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
