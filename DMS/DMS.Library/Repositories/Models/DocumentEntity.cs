using DMS.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    // Owner is at most one of step, contract, bill or form template (check constraint in configuration)
    [Table("documents")]
    [Index(nameof(StorageKey), IsUnique = true)]
    public class DocumentEntity : Entity
    {
        [MaxLength(255)]
        [Column("original_file_name")]
        public string OriginalFileName { get; set; }

        // Relative key, e.g. "TRX-2026-0142_Metalprom/04_Isporuka/otpremnica__c91d.pdf"
        [MaxLength(500)]
        [Column("storage_key")]
        public string StorageKey { get; set; }

        [MaxLength(100)]
        [Column("content_type")]
        public string ContentType { get; set; }

        [Column("size_bytes")]
        public long SizeBytes { get; set; }

        // All owners null for standalone documents in the archive
        [Column("step_id")]
        public long? StepId { get; set; }

        [ForeignKey(nameof(StepId))]
        public StepEntity? Step { get; set; }

        // Which requested document this file fulfills; null for an extra attachment in the step
        [Column("requirement_id")]
        public long? RequirementId { get; set; }

        [ForeignKey(nameof(RequirementId))]
        public DocumentRequirementEntity? Requirement { get; set; }

        [Column("contract_id")]
        public long? ContractId { get; set; }

        [ForeignKey(nameof(ContractId))]
        public ContractEntity? Contract { get; set; }

        [Column("bill_id")]
        public long? BillId { get; set; }

        [ForeignKey(nameof(BillId))]
        public BillEntity? Bill { get; set; }

        [Column("form_template_id")]
        public long? FormTemplateId { get; set; }

        [ForeignKey(nameof(FormTemplateId))]
        public FormTemplateEntity? FormTemplate { get; set; }
    }
}
