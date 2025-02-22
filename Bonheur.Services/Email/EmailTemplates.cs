using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Email
{
    public static class EmailTemplates
    {
        private static IWebHostEnvironment? _hostingEnvironment;
        private static string? confirmEmailTemplate;

        public static void Initialize(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public static string GetConfirmEmail(string recipientName, string callback)
        {
            confirmEmailTemplate ??= ReadPhysicalFile("Templates/ConfirmEmail.template");

            var emailMessage = confirmEmailTemplate
                .Replace("{{recipientName}}", recipientName)
                .Replace("{{callback}}", callback);

            return emailMessage;
        }

        public static string GetResetPasswordEmail(string recipientName, string callback)
        {
            confirmEmailTemplate ??= ReadPhysicalFile("Templates/ResetPasswordEmail.template");

            var emailMessage = confirmEmailTemplate
                .Replace("{{recipientName}}", recipientName)
                .Replace("{{callback}}", callback);

            return emailMessage;
        }

        public static string GetChangeEmail(string recipientName, string callback)
        {
            confirmEmailTemplate ??= ReadPhysicalFile("Templates/ChangeEmail.template");

            var emailMessage = confirmEmailTemplate
                .Replace("{{recipientName}}", recipientName)
                .Replace("{{callback}}", callback);

            return emailMessage;
        }

        public static string GetThankForPurchase(string recipientName, string spName, string website, string host)
        {
            confirmEmailTemplate ??= ReadPhysicalFile("Templates/ThankForPurchase.template");

            var emailMessage = confirmEmailTemplate
                .Replace("{{recipientName}}", recipientName)
                .Replace("{{spName}}", spName)
                .Replace("{{website}}", website)
                .Replace("{{host}}", host);

            return emailMessage;
        }

        public static string GetRequestToReview(string recipientName, string senderName, string host)
        {
            confirmEmailTemplate ??= ReadPhysicalFile("Templates/RequestReview.template");

            var emailMessage = confirmEmailTemplate
                .Replace("{{recipientName}}", recipientName)
                .Replace("{{senderName}}", senderName)
                .Replace("{{host}}", host);

            return emailMessage;
        }

        private static string ReadPhysicalFile(string path)
        {
            if (_hostingEnvironment == null)
                throw new InvalidOperationException($"{nameof(EmailTemplates)} is not initialized");

            var fileInfo = _hostingEnvironment.ContentRootFileProvider.GetFileInfo(path);

            if (!fileInfo.Exists)
                throw new FileNotFoundException($"Template file located at \"{path}\" was not found");

            using var fs = fileInfo.CreateReadStream();
            using var sr = new StreamReader(fs);
            return sr.ReadToEnd();
        }
    }
}
