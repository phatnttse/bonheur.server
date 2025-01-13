using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Bonheur.DAOs
{
    public class AdPackageDAO
    {
        private ApplicationDbContext _context;

        public AdPackageDAO(ApplicationDbContext adPackageDAO)
        {
            _context = adPackageDAO;
        }

        public async Task<AdPackage> AddAdPackage(AdPackage adPackage)
        {
            _context.AdPackages.Add(adPackage);
            await _context.SaveChangesAsync();
            return adPackage;
        }

        public async Task<IPagedList<AdPackage>> GetAdPackagesAsync(string adPackageTitle, int pageNumber = 1, int pageSize = 10)
        {
            IQueryable<AdPackage> query = _context.AdPackages;
            if (!string.IsNullOrEmpty(adPackageTitle))
            {
                query = query.Where(ap => EF.Functions.Like(ap.Title, $"%{ap.Title}%"));
            }
            var orderedQuery = query.OrderByDescending(x => x.CreatedAt);
            var adPackages = orderedQuery.ToPagedList(pageNumber, pageSize);
            return await Task.FromResult(adPackages);
        }

        public async Task<AdPackage?> GetAdPackageById(int id)
        {
            return await _context.AdPackages.FirstOrDefaultAsync(ap => ap.Id == id);
        }

        public async Task UpdateAdPackage(AdPackage adPackage) { 
            _context.AdPackages.Update(adPackage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAdPackage(AdPackage adPackage)
        {
            _context.AdPackages.Remove(adPackage);
            await _context.SaveChangesAsync();
        }
    }
}
