using DMS.Shared.Helper;
using System.Text.Json.Serialization;

namespace DMS.Shared.Constants
{
    [JsonConverter(typeof(StringEnumConverter<UtilityStatus>))]
    public enum UtilityStatus
    {
        Active = 0,
        Inactive = 1,
        Pending = 2
    }
}
