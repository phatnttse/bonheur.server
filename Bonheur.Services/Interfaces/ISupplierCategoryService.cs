using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.SupplierCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Bonheur.Services.Interfaces
{
    public interface ISupplierCategoryService
    {
        Task<ApplicationResponse> GetSupplierCategoryByIdAsync(int id);
        Task<ApplicationResponse> GetAllSupplierCategoryAsync();
        Task<ApplicationResponse> AddSupplierCategory(IFormFile file, string name, string description);
        Task<ApplicationResponse> UpdateSupplierCategory(IFormFile? file, string name, string description, int id);
        Task<ApplicationResponse> DeleteSupplierCategory(int id);
    }
}
