using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    // Gapless counter for codes like TRX-2026-0142, incremented with UPDATE ... RETURNING
    // in the same transaction that creates the record. Keyed by (prefix, year), so not an Entity.
    [Table("code_sequences")]
    [PrimaryKey(nameof(Prefix), nameof(Year))]
    public class CodeSequenceEntity
    {
        [MaxLength(10)]
        [Column("prefix")]
        public string Prefix { get; set; }

        // 0 for codes without a year
        [Column("year")]
        public int Year { get; set; }

        [Column("last_value")]
        public int LastValue { get; set; }
    }
}
