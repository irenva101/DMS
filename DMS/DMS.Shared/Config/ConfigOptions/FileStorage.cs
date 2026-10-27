using System.ComponentModel.DataAnnotations;

namespace DMS.Shared.Config.ConfigOptions
{
    public class FileStorage
    {
        // Each tenant gets its own folder below this root
        [Required(ErrorMessage = "FileStorage.RootPath is required.")]
        public string RootPath { get; set; }
    }
}
