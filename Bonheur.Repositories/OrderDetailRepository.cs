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
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly OrderDetailDAO _orderDetailDAO;

        public OrderDetailRepository(OrderDetailDAO orderDetailDAO)
        {
            _orderDetailDAO = orderDetailDAO;
        }

        public async Task<OrderDetail> AddOrderDetailAsync(OrderDetail orderDetail) => await _orderDetailDAO.AddOrderDetailAsync(orderDetail);
        public async Task<OrderDetail?> GetOrderDetailByIdAsync(int id) => await _orderDetailDAO.GetOrderDetailByIdAsync(id);
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId) => await _orderDetailDAO.GetOrderDetailsByOrderIdAsync(orderId);
        public async Task UpdateOrderDetailAsync(OrderDetail orderDetail) => await _orderDetailDAO.UpdateOrderDetailAsync(orderDetail);
        public async Task DeleteOrderDetailAsync(OrderDetail orderDetail) => await _orderDetailDAO.DeleteOrderDetailAsync(orderDetail);
    }
}
