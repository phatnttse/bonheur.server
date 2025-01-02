using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Bonheur.Services.DTOs.Storage;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Http;


namespace Bonheur.Services
{
    public class StorageService : IStorageService
    {
        private readonly string _azureStorageAccount = Environment.GetEnvironmentVariable("AZURE_BLOB_CONTAINER_NAME")!;

        private readonly string _azureStorageUrl = Environment.GetEnvironmentVariable("AZURE_BLOB_URL")!;

        private readonly string _azureStorageKey = Environment.GetEnvironmentVariable("AZURE_BLOB_KEY")!;

        private readonly BlobContainerClient _blobContainerClient;

        public StorageService()
        {
            var credentials = new StorageSharedKeyCredential
                (_azureStorageAccount, _azureStorageKey);
            var blobUri = new Uri(_azureStorageUrl);
            var blobServiceClient = new BlobServiceClient(blobUri, credentials);
            _blobContainerClient = blobServiceClient.GetBlobContainerClient("web");
        }

        public async Task<List<AzureBlobDTO>> ListAsync()
        {
            List<AzureBlobDTO> files = new List<AzureBlobDTO>();

            await foreach (var file in _blobContainerClient.GetBlobsAsync())
            {
                string uri = _blobContainerClient.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";

                files.Add(new AzureBlobDTO
                {
                    Uri = fullUri,
                    Name = name,
                    ContentType = file.Properties.ContentType
                });
            }

            return files;
        }

        public async Task<AzureBlobResponseDTO> UploadAsync(IFormFile blob)
        {
            AzureBlobResponseDTO response = new AzureBlobResponseDTO();

            string uniqueFileName = $"{DateTime.UtcNow.Ticks}_{blob.FileName}";

            BlobClient client = _blobContainerClient.GetBlobClient(uniqueFileName);

            await using (Stream? data = blob.OpenReadStream())
            {
                var headers = new BlobHttpHeaders
                {
                    ContentType = blob.ContentType
                };

                var uploadResponse = await client.UploadAsync(data, new BlobUploadOptions
                {
                    HttpHeaders = headers
                });

                if (uploadResponse.GetRawResponse().Status == 201)
                {
                    response.Status = $"File {blob.FileName} uploaded successfully";
                    response.Error = false;
                    response.Blob.Name = client.Name;
                    response.Blob.Uri = client.Uri.AbsoluteUri;
                }
                else
                {
                    response.Status = $"File {blob.FileName} upload failed with status code: {uploadResponse.GetRawResponse().Status}";
                    response.Error = true;
                }
            }

            return response;
        }


        public async Task<AzureBlobDTO?> DownloadAsync(string blobFileName)
        {
            BlobClient file = _blobContainerClient.GetBlobClient(blobFileName);

            if (await file.ExistsAsync())
            {
                var data = await file.OpenReadAsync();
                Stream blobContent = data;

                var content = await file.DownloadContentAsync();

                string name = blobFileName;
                string contentType = content.Value.Details.ContentType;

                return new AzureBlobDTO
                {
                    Name = name,
                    ContentType = contentType,
                    Content = blobContent
                };
            }
            return null;
        }

        public async Task<AzureBlobResponseDTO> DeleteAsync(string blobFileName)
        {

            BlobClient file = _blobContainerClient.GetBlobClient(blobFileName);

            var response = await file.DeleteIfExistsAsync();

            if (response.GetRawResponse().Status == 202)
            {
                return new AzureBlobResponseDTO
                {
                    Status = $"File {blobFileName} deleted successfully",
                    Error = false
                };
            }

            return new AzureBlobResponseDTO 
            {
                Status = $"File {blobFileName} delete failed",
                Error = true
            };
        }
    }
}
