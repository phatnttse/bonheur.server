using Microsoft.AspNetCore.Http;
using System.Globalization;
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
            // Chuyển thành chữ thường
            text = text.ToLower();

            // Chuẩn hóa tiếng Việt thành không dấu
            text = RemoveDiacritics(text);

            // Xóa các ký tự không mong muốn, giữ lại a-z, 0-9, khoảng trắng và dấu '-'
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

            // Thay nhiều khoảng trắng bằng 1 khoảng trắng
            text = Regex.Replace(text, @"\s+", " ").Trim();

            // Giới hạn độ dài slug
            text = text.Substring(0, text.Length <= 45 ? text.Length : 45).Trim();

            // Chuyển khoảng trắng thành dấu gạch ngang
            text = Regex.Replace(text, @"\s", "-");

            // Thêm timestamp
            string ticks = DateTime.Now.Ticks.ToString();

            return $"{text}_{ticks}";
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
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

        public static bool IsValidVideo(IFormFile file)
        {
            // Danh sách MIME types hợp lệ
            var allowedVideoTypes = new HashSet<string>
            {
                "video/mp4",
                "video/avi",
                "video/mpeg",
                "video/quicktime",
                "video/x-ms-wmv"
            };

            if (!allowedVideoTypes.Contains(file.ContentType)) return false;

            return true;

        }

        public static bool IsValidSizeFile(IFormFile file, int maxSize)
        {
            return file.Length <= maxSize;
        }


    }
}
