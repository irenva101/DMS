using DMS.Shared.Commons;
using DMS.Shared.Constants;

namespace DLMS.Tenants.Services.Models.Utility.DataIn
{
    public class UtilityGetAllDataIn
    {
        public required PageInfo PageInfo { get; set; }
        public string? Search { get; set; }
        public SortingType? Sorting { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public UtilityStatus? Status { get; set; }
    }
}
