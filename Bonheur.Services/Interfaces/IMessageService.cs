using Bonheur.BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IMessageService
    {
        ApplicationResponse GetSupplierMessageStatistics();
        Task<ApplicationResponse> GetUnreadMessagesCountByUser();
        Task<ApplicationResponse> UploadAttachmentFile(IFormFile file);
    }
}
