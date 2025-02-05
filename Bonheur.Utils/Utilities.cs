using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string GenerateSlug(string text)
        {
            text = text.ToLower();
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");  
            text = Regex.Replace(text, @"\s+", " ").Trim();   
            text = text.Substring(0, text.Length <= 45 ? text.Length : 45).Trim();  
            text = Regex.Replace(text, @"\s", "-");          
            string ticks = DateTime.Now.Ticks.ToString();     
            return $"{text}_{ticks}";
        }

        public static string NormalizeString(string input)
        {
            string normalized = input.Normalize(NormalizationForm.FormD);
            var noDiacritics = new string(normalized
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray());

            return noDiacritics.ToLower().Trim();
        }

        public static string FormatCurrency(decimal amount)
        {
            // Format number with thousands separators and append "đ"
            return $"{amount:N0}đ";
        }


    }
}
