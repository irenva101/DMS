using DMS.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    [Table("form_templates")]
    [Index(nameof(Code), IsUnique = true)]
    public class FormTemplateEntity : Entity
    {
        [MaxLength(30)]
        [Column("code")]
        public string Code { get; set; }

        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; }

        [MaxLength(50)]
        [Column("category")]
        public string Category { get; set; }

        // Each new version is a new document; the latest one is shown
        public List<DocumentEntity> Documents { get; set; }
    }
}
