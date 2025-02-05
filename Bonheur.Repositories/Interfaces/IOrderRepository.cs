using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order?> GetOrderByCodeAsync(int orderCode);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order> UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);

    }
}
