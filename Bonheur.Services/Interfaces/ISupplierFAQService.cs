using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.SupplierFAQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface ISupplierFAQService
    {
        Task<ApplicationResponse> GetSupplierFAQsBySupplier();
        Task<ApplicationResponse> CreateSupplierFAQs(List<SupplierFAQDTO> createSupplierFAQDTO);
        Task<ApplicationResponse> UpdateSupplierFAQs(List<SupplierFAQDTO> updateSupplierFAQDTO);
        Task<ApplicationResponse> DeleteSupplierFAQs(int id);
    }
}
