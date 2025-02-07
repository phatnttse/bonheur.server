using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.SocialNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface ISupplierSocialNetworkService
    {
        Task<ApplicationResponse> GetSocialNetworksBySupplier();
        Task<ApplicationResponse> CreateSupplierSocialNetwork(List<SupplierSocialNetworkDTO> createSupplierSocialNetworkDTO);
        Task<ApplicationResponse> UpdateSupplierSocialNetworks(List<SupplierSocialNetworkDTO> updateSupplierSocialNetworkDTO);
    }
}
