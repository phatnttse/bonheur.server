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

        public async Task<List<RequestPricing?>> GetAllRequestPricing(string currentUserId)
        {
            var result = await _context.RequestPricings
                .Where(rp => rp.Supplier != null &&  rp.Supplier.UserId == currentUserId)
                .ToListAsync();
            return result;
        }

        public async Task<RequestPricing> GetRequestPricingById(int id)
        {
            var result = await _context.RequestPricings
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
