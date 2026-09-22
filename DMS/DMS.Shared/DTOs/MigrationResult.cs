namespace DMS.Shared.DTOs
{
    public class MigrationResult
    {
        public string Context { get; set; }
        public string Acronym { get; set; }
        public bool Success { get; set; }
        public string? Error { get; set; }
    }
}
