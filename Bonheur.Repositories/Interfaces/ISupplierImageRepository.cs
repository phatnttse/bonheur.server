using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories.Interfaces
{
    public interface ISupplierImageRepository
    {
        Task AddSupplierImageAsync(SupplierImage supplierImage);
        Task AddSupplierImagesAsync(IEnumerable<SupplierImage> supplierImages);
        Task DeleteSupplierImageAsync(int id);
        Task DeleteSupplierImagesBySupplierIdAsync(int supplierId);
        Task<IEnumerable<SupplierImage>> GetSupplierImagesBySupplierIdAsync(int supplierId);
        Task<SupplierImage> GetSupplierImageByIdAsync(int id);
        Task UpdatePrimaryImageAsync(SupplierImage supplierImage);
        Task<SupplierImage> GetPrimaryImageBySupplierId(int supplierId);
    }
}
