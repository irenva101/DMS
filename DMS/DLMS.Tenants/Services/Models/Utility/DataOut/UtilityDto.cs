using DLMS.Tenants.Repositories.Models;
using DMS.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace DLMS.Tenants.Services.Models.Utility.DataOut
{
    public class UtilityDto
    {
        public Guid? Id { get; set; }
        [RegularExpression("^[a-zA-ZÀ-ž'\\- ]+$", ErrorMessage = "Only letters, spaces, apostrophes and hyphens are allowed.")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Acronym is required.")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Acronym { get; set; }
        [Required(ErrorMessage = "Contact person email is required.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        public string ContactPersonEmail { get; set; }
        [Required(ErrorMessage = "Contact person first name is required.")]
        public string ContactPersonFirstName { get; set; }
        [Required(ErrorMessage = "Contact person first name is required.")]
        public string ContactPersonLastName { get; set; }
        public Guid? ContactPerson { get; set; }
        public string? ContactPersonPhoneNumber { get; set; }
        public UtilityStatus Status { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Website Address is required.")]
        public string WebsiteAddress { get; set; }
        public DateTime CreationDate { get; set; }
        public string LogoPath { get; set; }
        public int TimeZone { get; set; }

        public UtilityDto()
        {

        }

        public UtilityDto(UtilityEntity x)
        {
            Name = x.Name;
            Id = x.DmsId;
            Acronym = x.Acronym;
            ContactPersonFirstName = x.ContactPerson != null ? x.ContactPerson.FirstName : null;
            ContactPersonLastName = x.ContactPerson != null ? x.ContactPerson.LastName : null;
            ContactPerson = x.ContactPerson != null ? x.ContactPerson.DmsId : null;
            ContactPersonEmail = x.ContactPerson != null ? x.ContactPerson.Email : null;
            ContactPersonPhoneNumber = x.ContactPerson != null ? x.ContactPerson.PhoneNumber : null;
            Status = x.Status;
            Address = x.Address;
            WebsiteAddress = x.WebsiteAddress;
            CreationDate = x.CreationDate;
            LogoPath = x.LogoPath;
            TimeZone = x.TimeZone;
        }
    }
}
