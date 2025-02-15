using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ApplicationResponse> GetOrderByCode(int orderCode);
        Task<ApplicationResponse> GetOrdersAsync(string? orderCode, OrderStatus? status, string? name, string? email, string? phone, string? address, string? province, string? ward, string? district, bool? sortAsc, string? orderBy, int pageNumber = 1, int pageSize = 10);
    }
}
