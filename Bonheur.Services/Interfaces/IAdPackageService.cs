using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.AdPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Services.Interfaces
{
    public interface IAdPackageService
    {
        Task<ApplicationResponse> AddAdPackage(AdPackageDTO adPackage);

        Task<ApplicationResponse> GetAdPackagesAsync(string? adPackageTitle, int pageNumber = 1, int pageSize = 10);

        Task<ApplicationResponse> GetAdPackageById(int id);

        Task<ApplicationResponse> UpdateAdPackage(int id, AdPackageDTO adPackage);

        Task<ApplicationResponse> DeleteAdPackage(int id);
    }
}
