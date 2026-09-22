using DLMS.Tenants.Repositories.Models;
using DMS.Shared.Constants;

namespace DLMS.Tenants.Services.Models.Role.DataOut
{
    public class RoleSimpleDataOut
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public UserStatus Status { get; set; }

        public RoleSimpleDataOut() { }

        public RoleSimpleDataOut(RoleEntity role)
        {
            Id = role.DmsId;
            Name = role.Name;
            Status = role.Status;
        }
    }
}
