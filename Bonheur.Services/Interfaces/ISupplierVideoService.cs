using Bonheur.BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface ISupplierVideoService
    {
        Task<ApplicationResponse> GetSupplierVideosBySupplierAsync();
        Task<ApplicationResponse> AddSupplierVideoAsync(IFormFile file);
        Task<ApplicationResponse> AddSupplierVideoRangeAsync(IEnumerable<IFormFile> files);
        Task<ApplicationResponse> DeleteSupplierVideoAsync(int id);
    }
}
