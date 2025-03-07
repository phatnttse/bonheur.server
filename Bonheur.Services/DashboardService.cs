using AutoMapper;
using Bonheur.BusinessObjects.Models;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Dashboard;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.EntityFrameworkCore;


namespace Bonheur.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IRequestPricingsRepository _requestPricingsRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public DashboardService(IUserAccountRepository userAccountRepository, IOrderRepository orderRepository, ISupplierRepository supplierRepository, IInvoiceRepository invoiceRepository, IAdvertisementRepository advertisementRepository, IMapper mapper, IRequestPricingsRepository requestPricingsRepository, ApplicationDbContext dbContext)
        {
            _userAccountRepository = userAccountRepository;
            _orderRepository = orderRepository;
            _supplierRepository = supplierRepository;
            _invoiceRepository = invoiceRepository;
            _advertisementRepository = advertisementRepository;
            _mapper = mapper;
            _requestPricingsRepository = requestPricingsRepository;
            _context = dbContext;
        }

        public async Task<ApplicationResponse> GetDashboardData()
        {
            try
            {              
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM GetDashboardSummary()"; // PostgreSQL function
                using var reader = await command.ExecuteReaderAsync();

                var result = new List<object[]>();

                while (await reader.ReadAsync())
                {
                    var row = new object[reader.FieldCount];
                    reader.GetValues(row);
                    result.Add(row);
                }

                if (result.Count == 0)
                {
                    return new ApplicationResponse
                    {
                        Data = null,
                        Message = "No data found",
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                var data = result.Select(row => new DashboardDataDTO
                {
                    TotalUsers = Convert.ToInt32(row[0]),
                    TotalOrders = Convert.ToInt32(row[1]),
                    TotalSuppliers = Convert.ToInt32(row[2]),
                    TotalInvoices = Convert.ToInt32(row[3]),
                    TotalAdvertisements = Convert.ToInt32(row[4]),
                    TotalRequestPricing = Convert.ToInt32(row[5]),
                    TotalRevenue = Convert.ToInt32(row[6])
                }).FirstOrDefault();

                return new ApplicationResponse
                {
                    Data = data,
                    Message = "Dashboard data retrieved successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true
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

        public async Task<ApplicationResponse> GetMonthlyDashboardData()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM GetMonthlyDashboardSummary()"; // PostgreSQL function
                using var reader = await command.ExecuteReaderAsync();

                var result = new List<object[]>();

                while (await reader.ReadAsync())
                {
                    var row = new object[reader.FieldCount];
                    reader.GetValues(row);
                    result.Add(row);
                }

                if (result.Count == 0)
                {
                    return new ApplicationResponse
                    {
                        Data = null,
                        Message = "No data found",
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                var data = result.Select(row => new DashboardMonthlyData
                {
                    Month = row[0].ToString(),
                    Year = Convert.ToInt32(row[1]),
                    TotalRevenue = Convert.ToDecimal(row[2]),
                    TotalOrders = Convert.ToInt32(row[3]),
                    TotalSuppliers = Convert.ToInt32(row[4])
                }).ToList();
               
                return new ApplicationResponse
                {
                    Data = data,
                    Message = "Monthly dashboard data retrieved successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true
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

        public async Task<ApplicationResponse> GetTopSuppliersByRevenue(int limit)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM GetTopSuppliersByRevenue({limit})"; // PostgreSQL function
                using var reader = await command.ExecuteReaderAsync();

                var result = new List<object[]>();

                while (await reader.ReadAsync())
                {
                    var row = new object[reader.FieldCount];
                    reader.GetValues(row);
                    result.Add(row);
                }

                if (result.Count == 0)
                {
                    return new ApplicationResponse
                    {
                        Data = null,
                        Message = "No data found",
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                var data = result.Select(row => new TopSuppliersByRevenue
                {
                    SupplierId = Convert.ToInt32(row[0]),
                    Name = row[1]?.ToString(),
                    PhoneNumber = row[2]?.ToString(),
                    Address = row[3]?.ToString(),
                    Street = row[4]?.ToString(),
                    District = row[5]?.ToString(),
                    Ward = row[6]?.ToString(),
                    Province = row[7]?.ToString(),
                    WebsiteUrl = row[8]?.ToString(),
                    AverageRating = Convert.ToDecimal(row[9]),
                    TotalRating = Convert.ToInt32(row[10]),
                    SubscriptionPackage = row[11]?.ToString(),
                    TotalPayment = Convert.ToDecimal(row[12]),
                    TotalCompletedOrders = Convert.ToInt32(row[13]),
                    PrimaryImageUrl = row[14]?.ToString()
                }).ToList();

                return new ApplicationResponse
                {
                    Data = data,
                    Message = "Top suppliers by revenue retrieved successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true
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
