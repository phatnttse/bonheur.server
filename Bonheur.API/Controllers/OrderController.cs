using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("/code/{orderCode}")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOrderByCode(int orderCode)
        {
            return Ok(await _orderService.GetOrderByCode(orderCode));
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetOrders(
            [FromQuery] string? orderCode, 
            [FromQuery] OrderStatus? status, 
            [FromQuery] string? name, 
            [FromQuery] string? email, 
            [FromQuery] string? phone, 
            [FromQuery] string? address, 
            [FromQuery] string? province, 
            [FromQuery] string? ward, 
            [FromQuery] string? district, 
            [FromQuery] bool? sortAsc, 
            [FromQuery] string? orderBy, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            return Ok(await _orderService.GetOrdersAsync(orderCode, status, name, email, phone, address, province, ward, district, sortAsc, orderBy, pageNumber, pageSize));
        }

    }
}
