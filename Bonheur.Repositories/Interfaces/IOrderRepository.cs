using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

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
        Task<IPagedList<Order>> GetOrdersAsync(string? orderCode, OrderStatus? status, string? name, string? email, string? phone, string? address, string? province, string? ward, string? district, bool? sortAsc, string? orderBy, int pageNumber = 1, int pageSize = 10);

    }
}
