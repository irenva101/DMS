using DMS.Shared.Commons;

namespace DLMS.Tenants.Services.Models.Permissions.DataIn
{
    public class PermissionGetAllDataIn
    {
        public required PageInfo PageInfo { get; set; }
        public string? Search { get; set; }
        public SortingType? Sorting { get; set; }
    }
}
