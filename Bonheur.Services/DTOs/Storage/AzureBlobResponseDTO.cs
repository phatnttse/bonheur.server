using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Storage
{
    public class AzureBlobResponseDTO
    {
        public AzureBlobResponseDTO()
        {
            Blob = new AzureBlobDTO();
        }

        public string? Status { get; set; }
        public bool Error { get; set; }
        public AzureBlobDTO Blob { get; set; }

    }
}
