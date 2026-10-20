using DMS.Shared.Constants;
using DMS.Shared.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    [Table("process_types")]
    public class ProcessTypeEntity : Entity
    {
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }

        // Which menu the process appears in
        [Column("module")]
        public ProcessModule Module { get; set; }

        // e.g. TRX, DIST-D, BNK-I
        [MaxLength(10)]
        [Column("code_prefix")]
        public string CodePrefix { get; set; }

        // Old templates are deactivated instead of deleted, so existing processes stay valid
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public List<StepDefinitionEntity> StepDefinitions { get; set; }
    }
}
