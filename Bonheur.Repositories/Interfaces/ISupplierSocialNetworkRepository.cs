using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface ISupplierSocialNetworkRepository
    {
        Task<IEnumerable<SupplierSocialNetwork>> GetAllAsync();
        Task<SupplierSocialNetwork?> GetByIdAsync(int id);
        Task<List<SupplierSocialNetwork>> GetSupplierSocialNetworksBySupplierIdAsync(int supplierId);
        Task<List<SupplierSocialNetwork>> GetBySocialNetworkIdsAsync(List<int> socialNetworkIds);
        Task<List<SupplierSocialNetwork>> CreateAsync(List<SupplierSocialNetwork> supplierSocialNetworks);
        Task<List<SupplierSocialNetwork>> UpdateAsync(List<SupplierSocialNetwork> supplierSocialNetwork);
        Task DeleteAsync(SupplierSocialNetwork supplierSocialNetwork);
    }
}
