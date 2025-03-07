using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Bonheur.DAOs
{
    public class OrderDAO
    {
        private readonly ApplicationDbContext _context;

        public OrderDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<IPagedList<Order>> GetOrdersAsync(
                string? orderCode,
                OrderStatus? status,
                string? name,
                string? email,
                string? phone,
                string? address,
                string? province,
                string? ward,
                string? district,
                bool? sortAsc,
                string? orderBy,
                int pageNumber = 1,
                int pageSize = 10)
        {
            var orders =  _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.User)
                .Include(o => o.Supplier)
                .Where(o => string.IsNullOrEmpty(orderCode) || o.OrderCode.ToString().Contains(orderCode))
                .Where(o => !status.HasValue || o.Status == status)
                .Where(o => string.IsNullOrEmpty(name) || o.Supplier!.Name!.ToLower().Contains(name.ToLower()))
                .Where(o => string.IsNullOrEmpty(email) || o.User!.Email!.ToLower().Contains(email.ToLower()))
                .Where(o => string.IsNullOrEmpty(phone) || o.Supplier!.PhoneNumber!.Contains(phone))
                .Where(o => string.IsNullOrEmpty(address) || o.Supplier!.Address!.ToLower().Contains(address.ToLower()))
                .Where(o => string.IsNullOrEmpty(province) || o.Supplier!.Province!.ToLower().Contains(province.ToLower()))
                .Where(o => string.IsNullOrEmpty(ward) || o.Supplier!.Ward!.Contains(ward))
                .Where(o => string.IsNullOrEmpty(district) || o.Supplier!.District!.ToLower().Contains(district.ToLower()))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(o => o.CreatedAt);

            if (!string.IsNullOrEmpty(orderBy))
            {
                // Tạo biểu thức cho orderBy
                var parameter = Expression.Parameter(typeof(Order), "s");
                var property = Expression.Property(parameter, orderBy);
                var lambda = Expression.Lambda<Func<Order, object>>(Expression.Convert(property, typeof(object)), parameter);

                // Áp dụng sắp xếp
                orders = sortAsc == true
                    ? orders.OrderBy(lambda)
                    : orders.OrderByDescending(lambda);
            }

            return Task.FromResult(orders.ToPagedList(pageNumber, pageSize));
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

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }
    }
}
