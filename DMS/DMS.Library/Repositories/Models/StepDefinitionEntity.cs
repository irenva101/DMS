using DMS.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    [Table("step_definitions")]
    [Index(nameof(ProcessTypeId), nameof(Order), IsUnique = true)]
    public class StepDefinitionEntity : Entity
    {
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }

        // Guidance for the user: what belongs in this step and why
        [MaxLength(1000)]
        [Column("description")]
        public string? Description { get; set; }

        [Column("step_order")]
        public int Order { get; set; }

        [Column("process_type_id")]
        public long ProcessTypeId { get; set; }

        [ForeignKey(nameof(ProcessTypeId))]
        public ProcessTypeEntity ProcessType { get; set; }

        // Empty for a free-form upload step
        public List<DocumentRequirementEntity> DocumentRequirements { get; set; }
    }
}
