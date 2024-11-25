using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<RequestPricing?>> GetAllRequestPricing(string supplierId)
        {
            var result = await _context.RequestPricings
                .Include(rp => rp.Supplier)
                .Where(rp => rp.Supplier != null &&  rp.Supplier.UserId == supplierId)
                .ToListAsync();
            return result;
        }

        public async Task<RequestPricing> GetRequestPricingById(string supplierId, int id)
        {
            var result = await _context.RequestPricings
                .Include(rp => rp.Supplier)
                .Where(rp => rp.Supplier != null && rp.Supplier.UserId == supplierId)
                .FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<RequestPricing> ChangeRequestPricingStatus(string supplierId, int id, RequestPricingStatus status)
        {
            var requestPricing = await _context.RequestPricings
                .Include(rp => rp.Supplier) 
                .FirstOrDefaultAsync(rp => rp.Supplier != null && rp.Supplier.UserId == supplierId && rp.Id == id);

            if (requestPricing == null)
            {
                throw new KeyNotFoundException("RequestPricing not found.");
            } 

            requestPricing.Status = status; 

            await _context.SaveChangesAsync();
            return requestPricing;
        }
    }
}
