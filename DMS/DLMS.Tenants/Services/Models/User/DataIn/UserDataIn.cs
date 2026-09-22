namespace DLMS.Tenants.Services.Models.User.DataIn
{
    public class UserDataIn
    {
        public Guid? Id { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public Guid Role { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Image { get; set; }
    }
}
