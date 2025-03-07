using Bonheur.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ApplicationResponse> GetDashboardData();
        Task<ApplicationResponse> GetMonthlyDashboardData();
        Task<ApplicationResponse> GetTopSuppliersByRevenue(int limit);
    }
}
