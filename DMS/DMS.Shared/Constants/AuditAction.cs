using System.Text.Json.Serialization;

namespace DMS.Shared.Constants
{
    [JsonConverter(typeof(JsonStringEnumConverter<AuditAction>))]
    public enum AuditAction
    {
        DocumentAttached = 0,
        DocumentDeleted = 1,
        StepVerified = 2,
        VerificationReset = 3,
        ProcessCompleted = 4,
        ProcessReopened = 5,
        ContractCreated = 6,
        ContractUpdated = 7,
        BillCreated = 8,
        BillUpdated = 9,
        ProcessCreated = 10,
    }
}
