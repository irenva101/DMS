using DMS.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    [Table("processes")]
    [Index(nameof(Code), IsUnique = true)]
    public class ProcessEntity : Entity
    {
        [MaxLength(30)]
        [Column("code")]
        public string Code { get; set; }

        [Column("company_id")]
        public long CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public CompanyEntity Company { get; set; }

        // Set on final confirmation; locks the process until an admin reopens it
        [Column("is_completed")]
        public bool IsCompleted { get; set; }

        // "Pošalji na SEF" for domestic distribution cases
        [Column("sent_to_sef")]
        public bool SentToSef { get; set; }

        [Column("process_type_id")]
        public long ProcessTypeId { get; set; }

        [ForeignKey(nameof(ProcessTypeId))]
        public ProcessTypeEntity ProcessType { get; set; }

        // e.g. a business transaction referencing its import process
        [Column("parent_process_id")]
        public long? ParentProcessId { get; set; }

        [ForeignKey(nameof(ParentProcessId))]
        public ProcessEntity? ParentProcess { get; set; }

        public List<StepEntity> Steps { get; set; }
    }
}
