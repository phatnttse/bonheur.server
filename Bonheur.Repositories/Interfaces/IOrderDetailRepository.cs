using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail> AddOrderDetailAsync(OrderDetail orderDetail);
        Task<OrderDetail?> GetOrderDetailByIdAsync(int id);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task UpdateOrderDetailAsync(OrderDetail orderDetail);
        Task DeleteOrderDetailAsync(OrderDetail orderDetail);
    }
}
