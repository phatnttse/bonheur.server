using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface IRequestPricingsRepository
    {
        Task<RequestPricing?> CreateRequestPricing(RequestPricing requestPricing);

        Task<List<RequestPricing?>> GetAllRequestPricing(string supplierId);

        Task<RequestPricing> GetRequestPricingById(string supplierId, int id);

        Task<RequestPricing?> ChangeRequestPricingStatus(string supplierId, int id, RequestPricingStatus status);
    }
}
