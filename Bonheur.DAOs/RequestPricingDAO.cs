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
            var result = _context.RequestPricings
                .Include(rp => rp.User)
                .Include(rp => rp.Supplier)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(rp => rp.CreatedAt)
                .ToPagedList(pageNumber, pageSize);

            return Task.FromResult(result);
        }

        public Task<IPagedList<RequestPricing>> GetAllRequestPricingBySupplierId(int supplierId, int pageNumber = 1, int pageSize = 10)
        {
            //IQueryable<RequestPricing> query = _context.RequestPricings
            //     .Where(rp => rp.SupplierId == supplierId);
            //var orderedQuery = query.OrderByDescending(rp => rp.CreatedAt);
            //var requestPricings = orderedQuery.ToPagedList(pageNumber, pageSize);
            //return Task.FromResult(requestPricings);

            var requestPricings = _context.RequestPricings
                 .Include(rp => rp.User)
                 .Where(rp => rp.SupplierId == supplierId)
                 .OrderByDescending(rp => rp.CreatedAt);

            var result = requestPricings.ToPagedList(pageNumber, pageSize);
            return Task.FromResult(result);
        }

        public async Task<List<RequestPricing>> GetRequestPricingsBySupplierId(int supplierId)
        {
            return await _context.RequestPricings
                 .Include(rp => rp.User)
                 .Where(rp => rp.SupplierId == supplierId)
                 .OrderByDescending(rp => rp.CreatedAt)
                 .ToListAsync();
        }

        public async Task<RequestPricing> GetRequestPricingById(int id)
        {
            var result = await _context.RequestPricings
                .Include(rp => rp.User)
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
