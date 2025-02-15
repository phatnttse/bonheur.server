using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.DTOs.Supplier;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Order
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public UserAccountDTO? User { get; set; }
        public SupplierDTO? Supplier { get; set; }
        public IEnumerable<OrderDetailDTO>? OrderDetails { get; set; }
    }
}
