using DMS.Shared.Constants;
using DMS.Shared.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DLMS.Tenants.Repositories.Models
{
    [Table("users")]
    public class UserEntity : Entity
    {
        [MaxLength(50)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Column("middle_name")]
        public string? MiddleName { get; set; }

        [MaxLength(50)]
        [Column("last_name")]
        public string LastName { get; set; }

        [Column("email")]
        [EmailAddress]
        [Required]
        public required string Email { get; set; }

        [Column("role_id")]
        public long? RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public RoleEntity? Role { get; set; }

        [Column("status")]
        public UserStatus Status { get; set; }

        [MaxLength(50)]
        [Column("address")]
        public string? Address { get; set; }

        [Column("phone_number")]
        public string? PhoneNumber { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        [Column("created_by")]
        public long? CreatedById { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public UserEntity CreatedBy { get; set; }

        [Column("created_time")]
        public DateTime CreatedTime { get; set; }

        [Column("utility_id")]
        public long? UtilityId { get; set; }

        [ForeignKey(nameof(UtilityId))]
        public UtilityEntity Utility { get; set; }

        [Column("is_admin")]
        public bool isAdmin { get; set; } = false;
    }
}
