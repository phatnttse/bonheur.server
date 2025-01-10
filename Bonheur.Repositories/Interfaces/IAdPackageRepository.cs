using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList.Extensions;
using X.PagedList;

namespace Bonheur.Repositories.Interfaces
{
    public interface IAdPackageRepository
    {

        Task<AdPackage> AddAdPackage(AdPackage adPackage);

        Task<IPagedList<AdPackage>> GetAdPackagesAsync(string adPackageTitle, int pageNumber = 1, int pageSize = 10);

        Task<AdPackage?> GetAdPackageById(int id);

        Task UpdateAdPackage(AdPackage adPackage);

        Task DeleteAdPackage(AdPackage adPackage);
    }
}
