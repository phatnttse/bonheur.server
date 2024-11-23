﻿using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Repositories
{
    public class SupplierImageRepository : ISupplierImageRepository
    {
        private readonly SupplierImageDAO _supplierImageDAO;

        public SupplierImageRepository(SupplierImageDAO supplierImageDAO)
        {
            _supplierImageDAO = supplierImageDAO;
        }

        public async Task AddSupplierImageAsync(SupplierImage supplierImage) => await _supplierImageDAO.AddSupplierImageAsync(supplierImage);
        public async Task AddSupplierImagesAsync(IEnumerable<SupplierImage> supplierImages) => await _supplierImageDAO.AddSupplierImagesAsync(supplierImages);
        public async Task DeleteSupplierImageAsync(int id) => await _supplierImageDAO.DeleteSupplierImageAsync(id);
        public async Task DeleteSupplierImagesBySupplierIdAsync(int supplierId) => await _supplierImageDAO.DeleteSupplierImagesBySupplierIdAsync(supplierId);
    }
}
