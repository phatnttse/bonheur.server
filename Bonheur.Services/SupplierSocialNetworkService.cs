using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.AdPackage;
using Bonheur.Services.DTOs.SocialNetwork;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services
{
    public class SupplierSocialNetworkService : ISupplierSocialNetworkService
    {
        private readonly ISupplierSocialNetworkRepository _supplierSocialNetworkRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISocialNetworkRepository _socialNetworkRepository;
        private readonly IMapper _mapper;

        public SupplierSocialNetworkService(ISupplierSocialNetworkRepository supplierSocialNetworkRepository, ISupplierRepository supplierRepository, IMapper mapper, ISocialNetworkRepository socialNetworkRepository)
        {
            _supplierSocialNetworkRepository = supplierSocialNetworkRepository;
            _supplierRepository = supplierRepository;
            _socialNetworkRepository = socialNetworkRepository;
            _mapper = mapper;
        }

        public async Task<ApplicationResponse> CreateSupplierSocialNetwork(List<SupplierSocialNetworkDTO> dtos)
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                Supplier supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId) ?? throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                List<SupplierSocialNetwork> existingSocialNetwork = await _supplierSocialNetworkRepository.GetBySocialNetworkIdsAsync(dtos.Select(d => d.SocialNetworkId!.Value).ToList());

                if (existingSocialNetwork.Count > 0) throw new ApiException("Social Network already exists", System.Net.HttpStatusCode.BadRequest);

                List<SocialNetwork> socialNetworks = await _socialNetworkRepository.GetByIdsAsync(dtos.Select(d => d.SocialNetworkId!.Value).ToList());

                if (socialNetworks.Count != dtos.Count) throw new ApiException("Social Network not found", System.Net.HttpStatusCode.NotFound);
                
                List<SupplierSocialNetwork> supplierSocialNetworks = new List<SupplierSocialNetwork>();

                _mapper.Map(dtos, supplierSocialNetworks);

                foreach (var item in supplierSocialNetworks)
                {
                    item.SupplierId = supplier.Id;
                    if (item.SupplierId == 0 || item.SocialNetworkId == 0) throw new ApiException("Invalid SupplierId or SocialNetworkId", System.Net.HttpStatusCode.BadRequest);
                }

                var createdData = await _supplierSocialNetworkRepository.CreateAsync(supplierSocialNetworks);

                return new ApplicationResponse
                {
                    Data = _mapper.Map<List<SupplierSocialNetworkDTO>>(createdData),
                    Message = "Supplier Social Network created successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetSocialNetworksBySupplier()
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                Supplier supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId) ?? throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                IEnumerable<SupplierSocialNetwork> supplierSocialNetworks = await _supplierSocialNetworkRepository.GetSupplierSocialNetworksBySupplierIdAsync(supplier.Id);

                return new ApplicationResponse
                {
                    Data = _mapper.Map<List<SupplierSocialNetworkDTO>>(supplierSocialNetworks),
                    Message = "Supplier Social Networks retrieved successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }


        public async Task<ApplicationResponse> UpdateSupplierSocialNetworks(List<SupplierSocialNetworkDTO> updateDtos)
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                Supplier supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId) ?? throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                List<SupplierSocialNetwork> existingSocialNetworks = await _supplierSocialNetworkRepository.GetSupplierSocialNetworksBySupplierIdAsync(supplier.Id);

                foreach (var dto in updateDtos)
                {
                    var socialNetwork = existingSocialNetworks.FirstOrDefault(s => s.Id == dto.Id);
                    if (socialNetwork != null)
                    {
                        _mapper.Map(dto, socialNetwork); 
                    }
                }

                var updatedSocialNetworks = await _supplierSocialNetworkRepository.UpdateAsync(existingSocialNetworks);

                return new ApplicationResponse
                {
                    Data = _mapper.Map<List<SupplierSocialNetworkDTO>>(updatedSocialNetworks),
                    Message = "Supplier Social Networks updated successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

    }
}
