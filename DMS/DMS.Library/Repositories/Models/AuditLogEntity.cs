using DMS.Shared.Constants;
using DMS.Shared.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    // Append-only: rows are never updated or deleted
    [Table("audit_logs")]
    public class AuditLogEntity : Entity
    {
        [Column("time")]
        public DateTime Time { get; set; }

        // Users live in the shared database, so this is the user's DmsId without a FK
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("action")]
        public AuditAction Action { get; set; }

        // Null for contract and bill actions
        [Column("process_id")]
        public long? ProcessId { get; set; }

        [ForeignKey(nameof(ProcessId))]
        public ProcessEntity? Process { get; set; }

        [Column("step_id")]
        public long? StepId { get; set; }

        [ForeignKey(nameof(StepId))]
        public StepEntity? Step { get; set; }

        [Column("document_id")]
        public long? DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public DocumentEntity? Document { get; set; }

        [Column("contract_id")]
        public long? ContractId { get; set; }

        [ForeignKey(nameof(ContractId))]
        public ContractEntity? Contract { get; set; }

        [Column("bill_id")]
        public long? BillId { get; set; }

        [ForeignKey(nameof(BillId))]
        public BillEntity? Bill { get; set; }

        // Snapshot at the time of the action, e.g. the file name
        [MaxLength(500)]
        [Column("description")]
        public string? Description { get; set; }
    }
}
