using Bonheur.BusinessObjects.Models;
using MimeKit;
using MailKit.Net.Smtp;
using Bonheur.Services.Interfaces;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Bonheur.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpConfig _config;

        public EmailSender()
        {
            _config = SmtpConfig.LoadFromEnvironment();
        }

        public async Task<(bool success, string? errorMsg)> SendEmailAsync(
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            bool isHtml = true)
        {
            var from = new MailboxAddress(_config.Name, _config.EmailAddress);
            var to = new MailboxAddress(recipientName, recipientEmail);

            return await SendEmailAsync(from, new[] { to }, subject, body, isHtml);
        }

        public async Task<(bool success, string? errorMsg)> SendEmailAsync(
            string senderName,
            string senderEmail,
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            bool isHtml = true)
        {
            var from = new MailboxAddress(senderName, senderEmail);
            var to = new MailboxAddress(recipientName, recipientEmail);

            return await SendEmailAsync(from, new[] { to }, subject, body, isHtml);
        }

        public async Task<(bool success, string? errorMsg)> SendEmailAsync(
            MailboxAddress sender,
            MailboxAddress[] recipients,
            string subject,
            string body,
            bool isHtml = true)
        {
            var message = new MimeMessage();

            message.From.Add(sender);
            message.To.AddRange(recipients);
            message.Subject = subject;
            message.Body = isHtml ?
                new BodyBuilder { HtmlBody = body }.ToMessageBody() :
                new TextPart("plain") { Text = body };

            try
            {
                using (var client = new SmtpClient())
                {
                    if (!_config.UseSSL)
                    {
                        client.ServerCertificateValidationCallback =
                        (sender2, certificate, chain, sslPolicyErrors) => true;
                    }
                    await client.ConnectAsync(_config.Host, _config.Port, _config.UseSSL).ConfigureAwait(false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    if (!string.IsNullOrWhiteSpace(_config.Username))
                        await client.AuthenticateAsync(_config.Username, _config.Password).ConfigureAwait(false);

                    await client.SendAsync(message).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool success, string? errorMsg)> SendEmailWithAttachmentAsync(
            string toEmail, string subject, string body, string attachmentUrl, string attachmentName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Bonheur", _config.Username));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = body
                };

                // Tải file PDF từ URL và đính kèm vào email
                using (var client = new HttpClient())
                {
                    var fileBytes = await client.GetByteArrayAsync(attachmentUrl);
                    builder.Attachments.Add(attachmentName, fileBytes, new ContentType("application", "pdf"));
                }

                message.Body = builder.ToMessageBody();

                using (var smtp = new SmtpClient())
                {
                    await smtp.ConnectAsync(_config.Host, _config.Port, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_config.Username, _config.Password);
                    await smtp.SendAsync(message);
                    await smtp.DisconnectAsync(true);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

    }
}
