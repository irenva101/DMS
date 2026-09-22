using DMS.Shared.Helper;
using System.Text.Json.Serialization;

namespace DMS.Shared.Commons
{
    [JsonConverter(typeof(StringEnumConverter<PermissionsEnum>))]
    public enum PermissionsEnum
    {
        #region Test
        test = 10000
        #endregion
    }
}
