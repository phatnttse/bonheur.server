﻿namespace Bonheur.API.Configurations
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
    }
}
