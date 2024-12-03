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

        Task<RequestPricing> GetRequestPricingById(int id);

        Task UpdateRequestPricingStatus(RequestPricing status);
    }
}
