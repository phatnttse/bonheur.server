using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface ISupplierFAQRepository
    {
        Task<IEnumerable<SupplierFAQ>> GetAllAsync();
        Task<SupplierFAQ?> GetByIdAsync(int id);
        Task<List<SupplierFAQ>> GetByIdsAsync(List<int> ids);
        Task<IEnumerable<SupplierFAQ>> GetSupplierFAQsBySupplierIdAsync(int supplierId);
        Task<List<SupplierFAQ>> CreateAsync(List<SupplierFAQ> supplierFAQs);
        Task<List<SupplierFAQ>> UpdateAsync(List<SupplierFAQ> supplierFAQs);
        Task DeleteAsync(SupplierFAQ supplierFAQ);
    }
}
