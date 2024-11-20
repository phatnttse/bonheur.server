using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.SupplierCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface ISupplierCategoryService
    {
        Task<ApplicationResponse> GetSupplierCategoryByIdAsync(int id);
        Task<ApplicationResponse> GetAllSupplierCategoryAsync();
        Task<ApplicationResponse> AddSupplierCategory(CreateSupplierCategoryDTO supplierCategoryDTO);
        Task<ApplicationResponse> UpdateSupplierCategory(CreateSupplierCategoryDTO supplierCategoryDTO, int id);
        Task<ApplicationResponse> DeleteSupplierCategory(int id);
    }
}
