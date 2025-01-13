﻿using Bonheur.BusinessObjects.Entities;
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
    public class FavoriteSupplierRepository : IFavoriteSupplierRepository
    {
        private readonly FavoriteSupplierDAO _favoriteSupplierDAO;

        public FavoriteSupplierRepository(FavoriteSupplierDAO favoriteSupplierDAO)
        {
            _favoriteSupplierDAO = favoriteSupplierDAO;
        }

        public Task<FavoriteSupplier> AddFavoriteSupplier(FavoriteSupplier favoriteSupplier) => _favoriteSupplierDAO.AddFavoriteSupplier(favoriteSupplier);

        public Task<FavoriteSupplier> DeleteSupplierAsync(FavoriteSupplier favoriteSupplier) => _favoriteSupplierDAO.DeleteSupplierAsync(favoriteSupplier);

        public Task<IPagedList<FavoriteSupplier>> GetAllFavoriteSuppliers(string userId, int pageNumber, int pageSize) => _favoriteSupplierDAO.GetAllFavoriteSuppliers(userId, pageNumber, pageSize);

        public Task<FavoriteSupplier?> GetFavoriteSupplierAsync(int id) => _favoriteSupplierDAO.GetFavoriteSupplierAsync(id);

        public Task<FavoriteSupplier> UpdateFavoriteSupplierAsync(FavoriteSupplier favoriteSupplier) => _favoriteSupplierDAO.UpdateFavoriteSupplierAsync(favoriteSupplier);
    }
}
