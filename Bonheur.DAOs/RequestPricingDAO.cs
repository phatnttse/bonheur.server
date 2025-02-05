using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Bonheur.DAOs
{
    public class RequestPricingDAO
    {
        private readonly ApplicationDbContext _context;
        public RequestPricingDAO(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<RequestPricing?> CreateRequestPricing(RequestPricing requestPricing) 
        {
             _context.RequestPricings.Add(requestPricing);
            await _context.SaveChangesAsync();
            return requestPricing;
        }

        public Task<IPagedList<RequestPricing>> GetAllRequestPricing(int pageNumber = 1, int pageSize= 10)
        {
            IQueryable<RequestPricing> query = _context.RequestPricings
                 .Where(rp => rp.Status != RequestPricingStatus.Rejected); 
            var orderedQuery =  query.OrderByDescending(rp => rp.CreatedAt);
            var requestPricings =  orderedQuery.ToPagedList(pageNumber, pageSize);
            return Task.FromResult(requestPricings);
        }

        public Task<IPagedList<RequestPricing>> GetAllRequestPricingBySupplierId(int supplierId, int pageNumber = 1, int pageSize = 10)
        {
            IQueryable<RequestPricing> query = _context.RequestPricings
                 .Where(rp => rp.Status != RequestPricingStatus.Rejected && rp.SupplierId == supplierId);
            var orderedQuery = query.OrderByDescending(rp => rp.CreatedAt);
            var requestPricings = orderedQuery.ToPagedList(pageNumber, pageSize);
            return Task.FromResult(requestPricings);
        }

        public async Task<RequestPricing> GetRequestPricingById(int id)
        {
            var result = await _context.RequestPricings
                .Include(rp=> rp.Supplier)
                .FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }


        public async Task UpdateRequestPricingStatus(RequestPricing requestPricing)
        {
            _context.RequestPricings.Update(requestPricing);
            await _context.SaveChangesAsync();
        }
    }
}
