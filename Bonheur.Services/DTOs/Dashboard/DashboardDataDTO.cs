using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Dashboard
{
    public class DashboardDataDTO
    {
        public int TotalUsers { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalAdvertisements { get; set; }
        public int TotalRequestPricing { get; set; }
        public int TotalRevenue { get; set; }
    }
}
