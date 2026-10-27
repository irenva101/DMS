using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DMS.Library.Helpers
{
    /// <summary>
    /// Builds the readable parts of a storage key, e.g. "Obrasci/OBR-001_Zahtev-za-godisnji-odmor/zahtev__b71e.docx".
    /// </summary>
    public static partial class StorageKey
    {
        private const int MaxPartLength = 60;

        // "Zahtev za godišnji odmor" -> "Zahtev-za-godisnji-odmor"
        public static string Sanitize(string value)
        {
            // đ has no decomposed form, so it is not stripped by the normalization below
            var decomposed = value.Replace("đ", "dj").Replace("Đ", "Dj").Normalize(NormalizationForm.FormD);

            var withoutMarks = new string(decomposed
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            var sanitized = NotAllowed().Replace(withoutMarks, "-").Trim('-');
            if (sanitized.Length > MaxPartLength)
                sanitized = sanitized[..MaxPartLength].TrimEnd('-');

            return sanitized.Length == 0 ? "fajl" : sanitized;
        }

        // Short random suffix, so two uploads with the same name never overwrite each other
        public static string FileName(string originalFileName)
        {
            var name = Sanitize(Path.GetFileNameWithoutExtension(originalFileName));
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            var suffix = Guid.NewGuid().ToString("N")[..4];

            return $"{name}__{suffix}{extension}";
        }

        [GeneratedRegex("[^A-Za-z0-9]+")]
        private static partial Regex NotAllowed();
    }
}
