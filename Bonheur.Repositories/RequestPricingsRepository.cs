using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories
{
    public class RequestPricingsRepository : IRequestPricingsRepository
    {
        private readonly RequestPricingDAO _requestPricingDAO;
        public RequestPricingsRepository(RequestPricingDAO requestPricingDAO) 
        { 
            _requestPricingDAO = requestPricingDAO;
        }

        public Task UpdateRequestPricingStatus(RequestPricing requestPricing)=> _requestPricingDAO.UpdateRequestPricingStatus(requestPricing); 

        public Task<RequestPricing?> CreateRequestPricing(RequestPricing requestPricing) => _requestPricingDAO.CreateRequestPricing(requestPricing);

        public async Task<IPagedList<RequestPricing?>> GetAllRequestPricing(int pageNumber=1, int pageSize=10) => await _requestPricingDAO.GetAllRequestPricing(pageNumber, pageSize);

        public Task<RequestPricing> GetRequestPricingById(int id) => _requestPricingDAO.GetRequestPricingById(id);

        public async Task<IPagedList<RequestPricing>> GetAllRequestPricingBySupplierId(int supplierId, int pageNumber = 1, int pageSize = 10) => await _requestPricingDAO.GetAllRequestPricingBySupplierId(supplierId,pageNumber, pageSize);
    }
}
