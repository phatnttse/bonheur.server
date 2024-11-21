using Bonheur.Services.DTOs.Storage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IStorageService
    {
        Task<List<AzureBlobDTO>> ListAsync();
        Task<AzureBlobResponseDTO> UploadAsync(IFormFile blob);
        Task<AzureBlobDTO?> DownloadAsync(string blobFileName);
        Task<AzureBlobResponseDTO> DeleteAsync(string blobFileName);
    }
}
