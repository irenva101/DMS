using DMS.Shared.Config.Config;
using DMS.Shared.Config.ConfigOptions;
using DMS.Shared.Helper;
using System.ComponentModel.DataAnnotations;

namespace DMS.Shared.Config
{
    public class DmsConfig
    {
        [ValidateObject()]
        public ConnectionStrings ConnectionStrings { get; set; }
        [Required(ErrorMessage = "Allowedhost is required.")]
        public string AllowedHosts { get; set; }
        [ValidateObject()]
        public LoggingConfig Logging { get; set; }
        public Otlp? Otlp { get; set; }
        [Required(ErrorMessage = "GITHUB_SHA is required.")]
        public string GITHUB_SHA { get; set; }
        [Required(ErrorMessage = "LongMigration is required.")]
        public bool LongMigration { get; set; }
    }
}
