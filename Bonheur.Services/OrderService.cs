using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Order;
using Bonheur.Services.DTOs.Supplier;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Bonheur.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<ApplicationResponse> GetOrderByCode(int orderCode)
        {
            try
            {
                if (!int.IsPositive(orderCode)) throw new ApiException("Invalid order code", System.Net.HttpStatusCode.BadRequest);

                Order? existingOrder = await _orderRepository.GetOrderByCodeAsync(orderCode);

                if (existingOrder == null) throw new ApiException("Order not found", System.Net.HttpStatusCode.NotFound);

                return new ApplicationResponse
                {
                    Data = _mapper.Map<OrderDTO>(existingOrder),
                    Message = "Order found",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetOrdersAsync(string? orderCode, OrderStatus? status, string? name, string? email, string? phone, string? address, string? province, string? ward, string? district, bool? sortAsc, string? orderBy, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                IPagedList<Order> ordersPagedList = await _orderRepository.GetOrdersAsync(orderCode, status, name, email, phone, address, province, ward, district, sortAsc, orderBy, pageNumber, pageSize);

                var ordersDTO = _mapper.Map<List<OrderDTO>>(ordersPagedList);

                var responseData = new PagedData<OrderDTO>
                {
                    Items = ordersDTO,
                    PageNumber = ordersPagedList.PageNumber,
                    PageSize = ordersPagedList.PageSize,
                    TotalItemCount = ordersPagedList.TotalItemCount,
                    PageCount = ordersPagedList.PageCount,
                    IsFirstPage = ordersPagedList.IsFirstPage,
                    IsLastPage = ordersPagedList.IsLastPage,
                    HasNextPage = ordersPagedList.HasNextPage,
                    HasPreviousPage = ordersPagedList.HasPreviousPage
                };

                return new ApplicationResponse
                {
                    Data = responseData,
                    Message = "Orders found",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
