using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.DAOs
{
    public class OrderDAO
    {
        private readonly ApplicationDbContext _context;

        public OrderDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.OrderDetails).Include(o => o.User).Include(o => o.Supplier).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetOrderByCodeAsync(int orderCode)
        {
            return await _context.Orders.Include(o => o.OrderDetails).Include(o => o.User).Include(o => o.Supplier).FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task DeleteOrderAsync(Order order)
        {
            _context.OrderDetails.RemoveRange(order.OrderDetails!);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
