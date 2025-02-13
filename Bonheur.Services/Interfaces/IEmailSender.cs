using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IEmailSender
    {
        Task<(bool success, string? errorMsg)> SendEmailAsync(
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            bool isHtml = true);

        Task<(bool success, string? errorMsg)> SendEmailAsync(
            string senderName,
            string senderEmail,
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            bool isHtml = true);

        Task<(bool success, string? errorMsg)> SendEmailWithAttachmentAsync(
            string toEmail, string subject, string body, string attachmentUrl, string attachmentName);
    }
}
