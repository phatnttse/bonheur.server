using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.RequestPricing;
using Bonheur.Services.DTOs.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IRequestPricingsService
    {
        Task<ApplicationResponse> CreateRequestPricing(CreateRequestPricingDTO requestPricing);

        Task<ApplicationResponse> GetAllRequestPricing();

        Task<ApplicationResponse> GetRequestPricingById(int id);

        Task<ApplicationResponse> UpdateRequestPricingStatus(int id, RequestPricingStatus status);

    }
}
