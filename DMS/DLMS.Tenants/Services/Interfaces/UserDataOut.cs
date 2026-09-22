using DLMS.Tenants.Repositories.Models;
using DMS.Shared.Constants;
using DMS.Shared.Helper;

namespace DLMS.Tenants.Services.Interfaces
{
    public class UserDataOut
    {
        public Guid DmsId { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Guid? RoleId { get; set; }
        public string? Role { get; set; }
        public UserStatus? RoleStatus { get; set; }
        public string Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string Image { get; set; }
        public List<Guid>? Areas { get; set; }
        public Guid? Region { get; set; }
        public DateTime CreatedTime { get; set; }
        public UserStatus Status { get; set; }
        public string CreatedByUserEmail { get; set; }
        public List<string>? AreaNames { get; set; }
        public string? RegionName { get; set; }
        public Guid UtilityId { get; set; }
        public string Utility { get; set; }
        public UtilityStatus? UtilityStatus { get; set; }
        public bool IsAdmin { get; set; }
        public int? TimeZone { get; set; }
        public List<Guid> RegionIds { get; set; }
        public bool AllRegions { get; set; }

        public UserDataOut() { }

        public UserDataOut(UserEntity user)
        {
            DmsId = user.DmsId;
            FirstName = user.FirstName;
            MiddleName = user.MiddleName;
            IsAdmin = user.isAdmin;
            LastName = user.LastName;
            Email = user.Email;
            Address = user.Address != null ? user.Address : HttpContextHelper.EmptyClaim;
            PhoneNumber = user.PhoneNumber;
            Image = user.Image != null ? user.Image : HttpContextHelper.EmptyClaim;
            RoleId = !user.isAdmin ? user.Role?.DmsId : Guid.Empty;
            Role = !user.isAdmin ? user.Role?.Name : "BPS Admin";
            CreatedTime = user.CreatedTime;
            Status = user.Status;
            RoleStatus = user.Role?.Status;
            UtilityId = user.Utility != null ? user.Utility.DmsId : Guid.Empty;
            Utility = user.Utility != null ? user.Utility.Acronym : HttpContextHelper.EmptyClaim;
            UtilityStatus = user.Utility != null ? user.Utility.Status : null;
            CreatedByUserEmail = user.CreatedBy != null && user.CreatedBy.Email != null ? user.CreatedBy.Email : HttpContextHelper.EmptyClaim;
            TimeZone = user.Utility != null ? user.Utility.TimeZone : null;
        }
    }
}
