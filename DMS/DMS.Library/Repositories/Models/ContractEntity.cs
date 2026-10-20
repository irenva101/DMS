using DMS.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    [Table("contracts")]
    [Index(nameof(Code), IsUnique = true)]
    public class ContractEntity : Entity
    {
        [MaxLength(30)]
        [Column("code")]
        public string Code { get; set; }

        [MaxLength(200)]
        [Column("title")]
        public string Title { get; set; }

        [Column("signed_date")]
        public DateOnly SignedDate { get; set; }

        // Null for open-ended contracts
        [Column("expiry_date")]
        public DateOnly? ExpiryDate { get; set; }

        [Precision(18, 2)]
        [Column("value")]
        public decimal? Value { get; set; }

        [MaxLength(3)]
        [Column("currency", TypeName = "char(3)")]
        public string Currency { get; set; }

        [Column("company_id")]
        public long CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public CompanyEntity Company { get; set; }

        // The contract also fulfills a step of some process
        [Column("fulfills_step_id")]
        public long? FulfillsStepId { get; set; }

        [ForeignKey(nameof(FulfillsStepId))]
        public StepEntity? FulfillsStep { get; set; }

        public List<DocumentEntity> Documents { get; set; }
    }
}
