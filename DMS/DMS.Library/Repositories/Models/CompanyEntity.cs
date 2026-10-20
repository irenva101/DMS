using DMS.Shared.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Library.Repositories.Models
{
    // Customers, suppliers and debtors the client does business with
    [Table("companies")]
    public class CompanyEntity : Entity
    {
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; }

        // PIB for domestic companies, VAT number for foreign ones
        [MaxLength(20)]
        [Column("tax_id")]
        public string? TaxId { get; set; }

        [MaxLength(20)]
        [Column("registration_number")]
        public string? RegistrationNumber { get; set; }

        // ISO 3166-1 alpha-2, "RS" for domestic companies
        [MaxLength(2)]
        [Column("country_code", TypeName = "char(2)")]
        public string CountryCode { get; set; }

        [MaxLength(300)]
        [Column("address")]
        public string? Address { get; set; }
    }
}
