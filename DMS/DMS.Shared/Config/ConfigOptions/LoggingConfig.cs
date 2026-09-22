using System.ComponentModel.DataAnnotations;

namespace DMS.Shared.Config.ConfigOptions
{
    public class LoggingConfig
    {
        [Required(ErrorMessage = "LogLevel is required.")]
        public Dictionary<string, string> LogLevel { get; set; }
    }
}
