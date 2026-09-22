using DMS.Shared.Commons;

namespace DLMS.Tenants.Services.Models.User.DataIn
{
    public class UserGetAllDataIn
    {
        public required PageInfo PageInfo { get; set; }
        public string? Search { get; set; }
        public SortingType? Sorting { get; set; }
    }
}
