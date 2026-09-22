using DLMS.Tenants.Repositories.Models;
using DMS.Shared.Commons;
using DMS.Shared.Constants;

namespace DLMS.Tenants.Services.Models.Role.DataOut
{
    public class RoleCrud
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedByUserEmail { get; set; }
        public List<PermissionsEnum> Permissions { get; set; }

        public RoleCrud() { }

        public RoleCrud(RoleEntity role)
        {
            Id = role.DmsId;
            Name = role.Name;
            CreatedTime = role.CreatedTime;
            Status = role.Status;
            CreatedByUserEmail = role.CreatedBy != null && (role.CreatedBy.Email != null) ? role.CreatedBy.Email : "/";
            Permissions = role.Permissions.Select(p => p.Value).ToList();
        }
    }
}
