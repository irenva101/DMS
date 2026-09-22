using DMS.Shared.Commons;
using DMS.Shared.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace DLMS.Tenants.Repositories.Models
{
    [Table("permissions")]
    public class PermissionEntity : Entity
    {
        [Column("value")]
        public PermissionsEnum Value { get; set; }
        public virtual List<RoleEntity>? Roles { get; set; }
    }
}
