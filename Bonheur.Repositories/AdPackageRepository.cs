using Bonheur.BusinessObjects.Entities;
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
    public class AdPackageRepository : IAdPackageRepository
    {
        private readonly AdPackageDAO _adPackageDAO;

        public AdPackageRepository(AdPackageDAO adPackageDAO)
        {
            _adPackageDAO = adPackageDAO;
        }
        public Task<AdPackage> AddAdPackage(AdPackage adPackage) => _adPackageDAO.AddAdPackage(adPackage);

        public Task DeleteAdPackage(AdPackage adPackage) => _adPackageDAO.DeleteAdPackage(adPackage);

        public Task<AdPackage?> GetAdPackageById(int id) => _adPackageDAO.GetAdPackageById(id);

        public Task<IPagedList<AdPackage>> GetAdPackagesAsync(string adPackageTitle, int pageNumber = 1, int pageSize = 10) => _adPackageDAO.GetAdPackagesAsync(adPackageTitle, pageNumber, pageSize);

        public Task UpdateAdPackage(AdPackage adPackage) => _adPackageDAO.UpdateAdPackage(adPackage);
    }
}
