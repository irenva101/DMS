using DMS.Shared.Helper;
using System.Text.Json.Serialization;

namespace DMS.Shared.Constants
{
    [JsonConverter(typeof(StringEnumConverter<ClockSetType>))]
    public enum ClockSetType : ushort
    {
        Unknown = 0,
        Utc = 1,
        Local = 2,
    }
}
