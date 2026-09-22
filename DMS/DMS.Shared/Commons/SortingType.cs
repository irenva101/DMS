using DMS.Shared.Helper;
using System.Text.Json.Serialization;

namespace DMS.Shared.Commons
{
    [JsonConverter(typeof(StringEnumConverter<SortingType>))]
    public enum SortingType
    {
        Ascending = 0,
        Descending = 1,
    }
}
