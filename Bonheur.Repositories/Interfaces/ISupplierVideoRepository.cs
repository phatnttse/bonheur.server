using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface ISupplierVideoRepository
    {
        Task<IEnumerable<SupplierVideo>> GetSupplierVideosBySupplierIdAsync(int supplierId);
        Task AddSupplierVideoAsync(SupplierVideo supplierVideo);
        Task AddSupplierVideoRangeAsync(IEnumerable<SupplierVideo> supplierVideos);
        Task DeleteSupplierVideoAsync(SupplierVideo supplierVideo);
        Task DeleteSupplierVideosBySupplierIdAsync(int supplierId);
        Task<SupplierVideo?> GetSupplierVideoById(int id);
    }
 
}
