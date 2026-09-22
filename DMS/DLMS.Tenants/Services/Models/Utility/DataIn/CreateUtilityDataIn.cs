using DLMS.Tenants.Repositories.Models;

namespace DLMS.Tenants.Services.Models.Utility.DataIn
{
    public class CreateUtilityDataIn
    {
        public string Name { get; set; }
        public string Acronym { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonFirstName { get; set; }
        public string ContactPersonLastName { get; set; }
        public string? ContactPersonPhoneNumber { get; set; }
        public string Address { get; set; }
        public string WebsiteAddress { get; set; }
        public int TimeZone { get; set; }

        public CreateUtilityDataIn()
        {

        }

        public CreateUtilityDataIn(UtilityEntity x)
        {
            Name = x.Name;
            Acronym = x.Acronym;
            ContactPersonFirstName = x.ContactPerson != null ? x.ContactPerson.FirstName : null;
            ContactPersonLastName = x.ContactPerson != null ? x.ContactPerson.LastName : null;
            ContactPersonEmail = x.ContactPerson != null ? x.ContactPerson.Email : null;
            ContactPersonPhoneNumber = x.ContactPerson != null ? x.ContactPerson.PhoneNumber : null;
            Address = x.Address;
            WebsiteAddress = x.WebsiteAddress;
            TimeZone = x.TimeZone;
        }
    }
}
