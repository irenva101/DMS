using DMS.Shared.Constants;
using DMS.Shared.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace DLMS.Tenants.Repositories.Models
{
    public class UtilityEntity : Entity
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("acronym")]
        public string Acronym { get; set; }

        [Column("contact_person")]
        public long? ContactPersonId { get; set; }

        [ForeignKey(nameof(ContactPersonId))]
        public UserEntity ContactPerson { get; set; }

        [Column("api_key")]
        public Guid ApiKey { get; set; }

        [Column("status")]
        public UtilityStatus Status { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("website_address")]
        public string WebsiteAddress { get; set; }
        [Column("time_zone")]
        public int TimeZone { get; set; }

        [Column("creation_date")]
        public DateTime CreationDate { get; set; }

        [Column("logo_path")]
        public string? LogoPath { get; set; }
    }
}
