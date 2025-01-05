using Bonheur.BusinessObjects.Models;
using MimeKit;
using MailKit.Net.Smtp;
using Bonheur.Services.Interfaces;
using MailKit.Security;

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
                    // Nếu không dùng SSL, bỏ qua kiểm tra chứng chỉ
                    if (!_config.UseSSL)
                    {
                        client.ServerCertificateValidationCallback =
                            (sender2, certificate, chain, sslPolicyErrors) => true;
                    }

                    // Sử dụng STARTTLS cho cổng 587
                    await client.ConnectAsync(_config.Host, 587, SecureSocketOptions.StartTls).ConfigureAwait(false);

                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Xác thực nếu có username và password
                    if (!string.IsNullOrWhiteSpace(_config.Username))
                        await client.AuthenticateAsync(_config.Username, _config.Password).ConfigureAwait(false);

                    // Gửi email
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

    }
}
