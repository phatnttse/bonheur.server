using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Supplier;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<ApplicationResponse> GetSupplierByIdAsync(int id);
        Task<ApplicationResponse> GetSupplierByUserIdAsync(string userId);
        Task<ApplicationResponse> GetSuppliersAsync(
                string? supplierName,
                int? supplierCategoryId,
                string? province,
                bool? isFeatured,
                decimal? averageRating,
                decimal? minPrice,
                decimal? maxPrice,
                bool? sortAsc,
                int pageNumber = 1,
                int pageSize = 10
            );
        Task<ApplicationResponse> CreateSupplierAsync(CreateSupplierDTO supplier);
        Task<ApplicationResponse> UpdateSupplierProfileAsync(UpdateSupplierProfileDTO supplier);
        Task<ApplicationResponse> UpdateSupplierAddressAsync(UpdateSupplierAddressDTO supplier);
        Task<ApplicationResponse> UploadSupplierImages(List<IFormFile> files, int? primaryImageIndex);
        Task<ApplicationResponse> GetSupplierImagesBySupplier(int supplierId);
        Task<ApplicationResponse> DeleteSupplierImageAsync(int supplierImageId);
        Task<ApplicationResponse> UpdatePrimaryImageAsync(int imageId);
        Task<ApplicationResponse> UpdateSupplierStatus(int supplierId, SupplierStatus status);
        Task<ApplicationResponse> GetSupplierBySlugAsync(string slug);
        Task<ApplicationResponse> ExportSupplierListToExcel(List<Supplier> suppliers);

    }
}
