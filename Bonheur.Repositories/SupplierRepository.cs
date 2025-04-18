﻿using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly SupplierDAO _supplierDAO;

        public SupplierRepository(SupplierDAO supplierDAO)
        {
            _supplierDAO = supplierDAO;
        }

        public async Task<Supplier?> CreateSupplierAsync(Supplier supplier) => await _supplierDAO.CreateSupplierAsync(supplier);

        public async Task<bool> DeleteSupplierAsync(int id) => await _supplierDAO.DeleteSupplierAsync(id);

        public async Task<Supplier?> GetSupplierByIdAsync(int id, bool isIncludeUser) => await _supplierDAO.GetSupplierByIdAsync(id, isIncludeUser);

        public async Task<Supplier?> GetSupplierByUserIdAsync(string userId) => await _supplierDAO.GetSupplierByUserIdAsync(userId);

        public async Task<IPagedList<Supplier>> GetSuppliersAsync(
                string? supplierName,
                List<int>? supplierCategoryIds,
                string? province,
                bool? isFeatured,
                decimal? averageRating,
                decimal? minPrice,
                decimal? maxPrice,
                bool? sortAsc,
                string? orderBy,
                int pageNumber = 1,
                int pageSize = 10
            ) => await _supplierDAO.GetSuppliersAsync(supplierName, supplierCategoryIds, province, isFeatured, averageRating, minPrice, maxPrice, sortAsc, orderBy, pageNumber, pageSize);

        public async Task<Supplier?> UpdateSupplierAsync(Supplier supplier) => await _supplierDAO.UpdateSupplierAsync(supplier);
        public async Task<bool> IsSupplierAsync(string userId) => await _supplierDAO.IsSupplierAsync(userId);
        public async Task<Supplier?> GetSupplierBySlugAsync(string slug) => await _supplierDAO.GetSupplierBySlugAsync(slug);      
        public async Task<List<Supplier>> GetAllSuppliersAsync() => await _supplierDAO.GetAllSuppliersAsync();
        public async Task<IPagedList<Supplier>> GetSuppliersByAdminAsync(
               string? supplierName,
               int? supplierCategoryId,
               string? province,
               bool? isFeatured,
               decimal? averageRating,
               decimal? minPrice,
               decimal? maxPrice,
               SupplierStatus? status,
               bool? sortAsc,
               string? orderBy,
               int pageNumber = 1,
               int pageSize = 10
           ) => await _supplierDAO.GetSuppliersByAdminAsync(supplierName, supplierCategoryId, province, isFeatured, averageRating, minPrice, maxPrice, status, sortAsc, orderBy, pageNumber, pageSize);

        public async Task<int> GetTotalSuppliersCountAsync() => await _supplierDAO.GetTotalSuppliersCountAsync();
    }
}
