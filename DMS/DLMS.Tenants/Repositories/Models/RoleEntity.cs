using DMS.Shared.Constants;
using DMS.Shared.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DLMS.Tenants.Repositories.Models
{
    [Table("roles")]
    public class RoleEntity : Entity
    {
        [MaxLength(30)]
        [Column("name")]
        public string Name { get; set; }
        public virtual List<PermissionEntity>? Permissions { get; set; }

        [Column("created_time")]
        public DateTime CreatedTime { get; set; }

        [Column("created_by")]
        public long? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public UserEntity CreatedBy { get; set; }

        [Column("status")]
        public UserStatus Status { get; set; }

        [Column("utility_id")]
        public long UtilityId { get; set; }

        [ForeignKey(nameof(UtilityId))]
        public UtilityEntity Utility { get; set; }
        public List<UserEntity> Users { get; set; }
    }
}
