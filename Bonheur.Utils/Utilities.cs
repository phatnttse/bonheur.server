using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Bonheur.Utils
{
    public static class Utilities
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static void QuickLog(string text, string logPath)
        {
            var dirPath = Path.GetDirectoryName(logPath);

            if (string.IsNullOrWhiteSpace(dirPath))
                throw new ArgumentException($"Specified path \"{logPath}\" is invalid", nameof(logPath));

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            using var writer = File.AppendText(logPath);
            writer.WriteLine($"{DateTime.Now} - {text}");
        }

        public static string? GetCurrentUserId() => _httpContextAccessor?.HttpContext?.User.FindFirstValue(Claims.Subject);

        public static string[] GetRoles(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == Claims.Role)
                .Select(c => c.Value)
                .ToArray();
        }
    }
}
