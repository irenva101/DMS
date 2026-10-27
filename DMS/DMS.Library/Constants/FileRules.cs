namespace DMS.Library.Constants
{
    public static class FileRules
    {
        // Also has to fit the client_max_body_size of both nginx instances
        public const long MaxFileSizeBytes = 50 * 1024 * 1024;

        public static readonly HashSet<string> AllowedExtensions =
        [
            ".pdf",
            ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".odt", ".ods",
            ".csv", ".txt", ".xml",
            ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tif", ".tiff",
            ".zip", ".rar", ".7z",
            ".msg", ".eml",
        ];
    }
}
