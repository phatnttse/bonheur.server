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
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDAO _orderDAO;

        public OrderRepository(OrderDAO orderDAO)
        {
            _orderDAO = orderDAO;
        }

        public async Task<Order> AddOrderAsync(Order order) => await _orderDAO.AddOrderAsync(order);
        public async Task<Order?> GetOrderByIdAsync(int id) => await _orderDAO.GetOrderByIdAsync(id);
        public async Task<IPagedList<Order>> GetOrdersAsync(string? orderCode, OrderStatus? status, string? name, string? email, string? phone, string? address, string? province, string? ward, string? district, bool? sortAsc, string? orderBy, int pageNumber = 1, int pageSize = 10) => await _orderDAO.GetOrdersAsync(orderCode, status, name, email, phone, address, province, ward, district, sortAsc, orderBy, pageNumber, pageSize);
        public async Task<Order?> GetOrderByCodeAsync(int orderCode) => await _orderDAO.GetOrderByCodeAsync(orderCode);
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId) => await _orderDAO.GetOrdersByUserIdAsync(userId);
        public async Task<Order> UpdateOrderAsync(Order order) => await _orderDAO.UpdateOrderAsync(order);
        public async Task DeleteOrderAsync(Order order) => await _orderDAO.DeleteOrderAsync(order);

        public async Task<int> GetTotalOrdersCountAsync() => await _orderDAO.GetTotalOrdersCountAsync();

    }
}
