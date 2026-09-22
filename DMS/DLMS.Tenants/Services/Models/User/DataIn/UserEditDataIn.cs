namespace DLMS.Tenants.Services.Models.User.DataIn
{
    public class UserEditDataIn
    {
        public Guid? Id { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public Guid Role { get; set; }
        public string? Address { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Image { get; set; }
    }
}
