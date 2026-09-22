using DLMS.Tenants.Repositories.Models;

namespace DLMS.Tenants.Services.Models.Utility.DataIn
{
    public class EditUtilityDataIn
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public Guid? ContactPerson { get; set; }
        public string Address { get; set; }
        public string WebsiteAddress { get; set; }
        public int TimeZone { get; set; }

        public EditUtilityDataIn()
        {

        }

        public EditUtilityDataIn(UtilityEntity x)
        {
            Name = x.Name;
            Id = x.DmsId;
            Acronym = x.Acronym;
            ContactPerson = x.ContactPerson != null ? x.ContactPerson.DmsId : null;
            Address = x.Address;
            WebsiteAddress = x.WebsiteAddress;
            TimeZone = x.TimeZone;
        }
    }
}
