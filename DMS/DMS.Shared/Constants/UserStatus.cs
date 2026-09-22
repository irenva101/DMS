using System.Text.Json.Serialization;

namespace DMS.Shared.Constants
{
    [JsonConverter(typeof(JsonStringEnumConverter<UserStatus>))]
    public enum UserStatus
    {
        Inactive = 0,
        Pending = 1,
        Active = 2,
    }
}
