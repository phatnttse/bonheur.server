using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface ISocialNetworkService
    {
        Task<ApplicationResponse> GetSocialNetworks();
        Task<ApplicationResponse> GetSocialNetworkById(int id);
        Task<ApplicationResponse> CreateSocialNetwork(IFormFile file, string name);
        Task<ApplicationResponse> UpdateSocialNetwork(IFormFile file, string name, int id);
        Task<ApplicationResponse> DeleteSocialNetwork(int id);
    }
}
