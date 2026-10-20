using System.Text.Json.Serialization;

namespace DMS.Shared.Constants
{
    [JsonConverter(typeof(JsonStringEnumConverter<ProcessModule>))]
    public enum ProcessModule
    {
        BusinessTransaction = 0,
        Import = 1,
        Distribution = 2,
        Export = 3,
        Bank = 4,
        BillOfExchange = 5,
    }
}
