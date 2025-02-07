using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.SupplierFAQ;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;


namespace Bonheur.Services
{
    public class SupplierFAQService : ISupplierFAQService
    {
        private readonly ISupplierFAQRepository _supplierFAQRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierFAQService(ISupplierFAQRepository supplierFAQRepository, ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierFAQRepository = supplierFAQRepository;
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<ApplicationResponse> CreateSupplierFAQs(List<SupplierFAQDTO> dtos)
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                Supplier supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId) ?? throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                List<SupplierFAQ> supplierFAQs = new List<SupplierFAQ>();

                _mapper.Map(dtos, supplierFAQs);

                foreach (var item in supplierFAQs)
                {
                    item.SupplierId = supplier.Id;
                }

                return new ApplicationResponse 
                {
                    Message = "Create supplier FAQ successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                    Data = _mapper.Map<List<SupplierFAQDTO>>(await _supplierFAQRepository.CreateAsync(supplierFAQs)),
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

        public async Task<ApplicationResponse> DeleteSupplierFAQs(int id)
        {

            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                Supplier supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId) ?? throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                SupplierFAQ supplierFAQ = await _supplierFAQRepository.GetByIdAsync(id) ?? throw new ApiException("Supplier FAQ not found", System.Net.HttpStatusCode.NotFound);

                await _supplierFAQRepository.DeleteAsync(supplierFAQ);

                return new ApplicationResponse
                {
                    Message = "Delete supplier FAQ successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                    Data = _mapper.Map<SupplierFAQDTO>(supplierFAQ),
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

        public async Task<ApplicationResponse> GetSupplierFAQsBySupplier()
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                Supplier supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId) ?? throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);
             
                return new ApplicationResponse
                {
                    Message = "Get Supplier FAQs Successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                    Data = _mapper.Map<List<SupplierFAQDTO>>(await _supplierFAQRepository.GetSupplierFAQsBySupplierIdAsync(supplier.Id)),
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

        public async Task<ApplicationResponse> UpdateSupplierFAQs(List<SupplierFAQDTO> dtos)
        {
            try
            {
                string currentUserId = Utilities.GetCurrentUserId() ?? throw new ApiException("User not found", System.Net.HttpStatusCode.NotFound);

                Supplier supplier = await _supplierRepository.GetSupplierByUserIdAsync(currentUserId) ?? throw new ApiException("Supplier not found", System.Net.HttpStatusCode.NotFound);

                List<SupplierFAQ> supplierFAQs = await _supplierFAQRepository.GetByIdsAsync(dtos.Select(d => d.Id!.Value).ToList());

                if (supplierFAQs.Count != dtos.Count) throw new ApiException("Supplier FAQ not found", System.Net.HttpStatusCode.NotFound);

                _mapper.Map(dtos, supplierFAQs);

                foreach (var faq in supplierFAQs)
                {
                    faq.SupplierId = supplier.Id;
                }

                return new ApplicationResponse
                {
                    Message = "Update supplier FAQ successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                    Data = _mapper.Map<List<SupplierFAQDTO>>(await _supplierFAQRepository.UpdateAsync(supplierFAQs)),
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
