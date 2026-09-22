using DMS.Shared.Commons;

namespace DLMS.Tenants.Services.Models.Role.DataIn
{
    public class RoleDataIn
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public List<PermissionsEnum>? Permissions { get; set; }
    }
}
