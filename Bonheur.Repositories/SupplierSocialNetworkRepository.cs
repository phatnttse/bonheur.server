using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories
{
    public class SupplierSocialNetworkRepository : ISupplierSocialNetworkRepository
    {
        private readonly SupplierSocialNetworkDAO _dao;

        public SupplierSocialNetworkRepository(SupplierSocialNetworkDAO dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<SupplierSocialNetwork>> GetAllAsync() => await _dao.GetAllAsync();

        public async Task<SupplierSocialNetwork?> GetByIdAsync(int id) => await _dao.GetByIdAsync(id);

        public async Task<List<SupplierSocialNetwork>> GetSupplierSocialNetworksBySupplierIdAsync(int supplierId) => await _dao.GetSupplierSocialNetworksBySupplierIdAsync(supplierId);

        public async Task<List<SupplierSocialNetwork>> GetBySocialNetworkIdsAsync(List<int> socialNetworkIds) => await _dao.GetBySocialNetworkIdsAsync(socialNetworkIds);

        public async Task<List<SupplierSocialNetwork>> CreateAsync(List<SupplierSocialNetwork> supplierSocialNetworks) => await _dao.CreateAsync(supplierSocialNetworks);

        public async Task<List<SupplierSocialNetwork>> UpdateAsync(List<SupplierSocialNetwork> supplierSocialNetwork) => await _dao.UpdateAsync(supplierSocialNetwork);

        public async Task DeleteAsync(SupplierSocialNetwork supplierSocialNetwork) => await _dao.DeleteAsync(supplierSocialNetwork);

    }
}
