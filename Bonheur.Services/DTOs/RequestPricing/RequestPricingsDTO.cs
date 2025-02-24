using Bonheur.BusinessObjects.Enums;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.DTOs.Supplier;

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
        public virtual UserAccountDTO? User { get; set; }
        public virtual SupplierDTO? Supplier { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? RejectReason { get; set; }
    }
}
