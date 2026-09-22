using DMS.Shared.Commons;

namespace DLMS.Tenants.Services.Models.Role.DataIn
{
    public class RoleGetAllDataIn
    {
        public long? userId { get; set; }
        public required PageInfo PageInfo { get; set; }
        public string? Search { get; set; }
        public SortingType? Sorting { get; set; }
    }
}
