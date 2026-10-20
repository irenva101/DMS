using DMS.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    [Table("steps")]
    [Index(nameof(ProcessId), nameof(StepDefinitionId), IsUnique = true)]
    public class StepEntity : Entity
    {
        // Reset to false whenever a document is added to or removed from the step
        [Column("is_verified")]
        public bool IsVerified { get; set; }

        [Column("process_id")]
        public long ProcessId { get; set; }

        [ForeignKey(nameof(ProcessId))]
        public ProcessEntity Process { get; set; }

        [Column("step_definition_id")]
        public long StepDefinitionId { get; set; }

        [ForeignKey(nameof(StepDefinitionId))]
        public StepDefinitionEntity StepDefinition { get; set; }

        public List<DocumentEntity> Documents { get; set; }
    }
}
