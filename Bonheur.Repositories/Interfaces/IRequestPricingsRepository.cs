using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories.Interfaces
{
    public interface IRequestPricingsRepository
    {
        Task<RequestPricing?> CreateRequestPricing(RequestPricing requestPricing);

        Task<IPagedList<RequestPricing?>> GetAllRequestPricing(int pageNumber=1, int pageSize=10);
        
        Task<IPagedList<RequestPricing>> GetAllRequestPricingBySupplierId(int supplierId, int pageNumber = 1, int pageSize=10);

        Task<RequestPricing> GetRequestPricingById(int id);

        Task UpdateRequestPricingStatus(RequestPricing status);
    }
}
