using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDAO _orderDAO;

        public OrderRepository(OrderDAO orderDAO)
        {
            _orderDAO = orderDAO;
        }

        public async Task<Order> AddOrderAsync(Order order) => await _orderDAO.AddOrderAsync(order);
        public async Task<Order?> GetOrderByIdAsync(int id) => await _orderDAO.GetOrderByIdAsync(id);
        public async Task<Order?> GetOrderByCodeAsync(int orderCode) => await _orderDAO.GetOrderByCodeAsync(orderCode);
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId) => await _orderDAO.GetOrdersByUserIdAsync(userId);
        public async Task<Order> UpdateOrderAsync(Order order) => await _orderDAO.UpdateOrderAsync(order);
        public async Task DeleteOrderAsync(Order order) => await _orderDAO.DeleteOrderAsync(order);

    }
}
