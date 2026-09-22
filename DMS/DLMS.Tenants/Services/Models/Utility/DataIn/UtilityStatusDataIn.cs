using DMS.Shared.Constants;

namespace DLMS.Tenants.Services.Models.Utility.DataIn
{
    public class UtilityStatusDataIn
    {
        public Guid UtilityId { get; set; }
        public UtilityStatus Status { get; set; }
    }
}
