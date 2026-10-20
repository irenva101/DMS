using DMS.Shared.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    // A named document a step asks for, e.g. "CMR" or "Nalog 70"
    [Table("document_requirements")]
    public class DocumentRequirementEntity : Entity
    {
        [MaxLength(150)]
        [Column("name")]
        public string Name { get; set; }

        [Column("sort_order")]
        public int SortOrder { get; set; }

        // If false, the step can be verified without it
        [Column("is_required")]
        public bool IsRequired { get; set; }

        [Column("step_definition_id")]
        public long StepDefinitionId { get; set; }

        [ForeignKey(nameof(StepDefinitionId))]
        public StepDefinitionEntity StepDefinition { get; set; }
    }
}
