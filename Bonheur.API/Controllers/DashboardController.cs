using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardAsync()
        {
            var dashboard = await _dashboardService.GetDashboardData();

            return Ok(dashboard);
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyDashboardData()
        {
            var dashboard = await _dashboardService.GetMonthlyDashboardData();

            return Ok(dashboard);
        }

        [HttpGet("top-suppliers")]
        public async Task<IActionResult> GetTopSuppliersByRevenue([FromQuery] int limit)
        {
            var dashboard = await _dashboardService.GetTopSuppliersByRevenue(limit);

            return Ok(dashboard);
        }
    }
}
