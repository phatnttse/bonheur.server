using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Dashboard
{
    public class DashboardMonthlyData
    {
        public string? Month { get; set; }
        public int Year { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalSuppliers { get; set; }
    }
}
