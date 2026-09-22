using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Shared.Repositories
{
    public class Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("dms_id")]
        public Guid DmsId { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("last_update_time")]
        public DateTime? LastUpdateTime { get; set; }

        public Entity()
        {
            DmsId = Guid.NewGuid();
        }
    }
}
