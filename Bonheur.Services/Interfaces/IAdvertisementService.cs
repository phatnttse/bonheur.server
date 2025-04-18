﻿using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Advertisement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IAdvertisementService
    {
        Task<ApplicationResponse> GetAdvertisementsAsync(string? searchTitle, string? searchContent, AdvertisementStatus? status, PaymentStatus? paymentStatus, int pageNumber = 1, int pageSize = 10);
        Task<ApplicationResponse> GetAdvertisementByIdAsync(int id);
        Task<ApplicationResponse> AddAdvertisementAsync(CreateAdvertisementDTO request);
        Task<ApplicationResponse> UpdateAdvertisementAsync(int id, CreateAdvertisementDTO advertisementDTO);
        Task<ApplicationResponse> DeleteAdvertisementAsync(int id);
        Task<ApplicationResponse> GetAdvertisementBySupplierAsync(int pageNumber, int pageSize);
        Task<ApplicationResponse> GetActiveAdvertisementsAsync(int pageNumber, int pageSize);
    }
}
