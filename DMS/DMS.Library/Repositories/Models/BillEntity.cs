using DMS.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    // Bill of exchange (menica)
    [Table("bills")]
    [Index(nameof(Code), IsUnique = true)]
    [Index(nameof(ProcessId), IsUnique = true)]
    public class BillEntity : Entity
    {
        [MaxLength(30)]
        [Column("code")]
        public string Code { get; set; }

        [Precision(18, 2)]
        [Column("amount")]
        public decimal Amount { get; set; }

        [MaxLength(3)]
        [Column("currency", TypeName = "char(3)")]
        public string Currency { get; set; }

        [Column("issue_date")]
        public DateOnly IssueDate { get; set; }

        [Column("maturity_date")]
        public DateOnly MaturityDate { get; set; }

        // Debtor
        [Column("company_id")]
        public long CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public CompanyEntity Company { get; set; }

        // Document flow of this bill, if one was started
        [Column("process_id")]
        public long? ProcessId { get; set; }

        [ForeignKey(nameof(ProcessId))]
        public ProcessEntity? Process { get; set; }

        public List<DocumentEntity> Documents { get; set; }
    }
}
