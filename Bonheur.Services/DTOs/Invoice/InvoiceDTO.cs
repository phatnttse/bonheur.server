using Bonheur.BusinessObjects.Entities;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.DTOs.Order;
using Bonheur.Services.DTOs.Supplier;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Invoice
{
    public class InvoiceDTO
    {
        public int Id { get; set; }

        public int InvoiceNumber { get; set; }

        public string? Description { get; set; }

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
