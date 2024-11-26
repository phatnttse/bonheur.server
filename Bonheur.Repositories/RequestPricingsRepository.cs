using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories
{
    public class RequestPricingsRepository : IRequestPricingsRepository
    {
        private readonly RequestPricingDAO _requestPricingDAO;
        public RequestPricingsRepository(RequestPricingDAO requestPricingDAO) 
        { 
            _requestPricingDAO = requestPricingDAO;
        }

        public Task<RequestPricing> ChangeRequestPricingStatus(string supplierId, int id, RequestPricingStatus status)=> _requestPricingDAO.ChangeRequestPricingStatus(supplierId, id, status); 

        public Task<RequestPricing?> CreateRequestPricing(RequestPricing requestPricing) => _requestPricingDAO.CreateRequestPricing(requestPricing);

        public Task<List<RequestPricing?>> GetAllRequestPricing(string supplierId) => _requestPricingDAO.GetAllRequestPricing(supplierId);

        public Task<RequestPricing> GetRequestPricingById(string supplierId, int id) => _requestPricingDAO.GetRequestPricingById(supplierId, id);
    }
}
