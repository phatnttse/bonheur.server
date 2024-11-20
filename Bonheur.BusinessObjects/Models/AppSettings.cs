namespace Bonheur.BusinessObjects.Models
{

    public class AppSettings
    {
        public SmtpConfig? SmtpConfig { get; set; }
    }

    public class SmtpConfig
    {
        public required string Host { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public required string EmailAddress { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        public static SmtpConfig LoadFromEnvironment()
        {
            return new SmtpConfig
            {
                Host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? throw new InvalidOperationException("SMTP_HOST is not defined in environment variables."),
                Port = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 25,
                UseSSL = bool.TryParse(Environment.GetEnvironmentVariable("SMTP_USESSL"), out var useSSL) ? useSSL : false,
                EmailAddress = Environment.GetEnvironmentVariable("SMTP_EMAILADDRESS") ?? throw new InvalidOperationException("SMTP_EMAILADDRESS is not defined in environment variables."),
                Name = Environment.GetEnvironmentVariable("SMTP_NAME"),
                Username = Environment.GetEnvironmentVariable("SMTP_USERNAME"),
                Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD")
            };
        }
    }
}