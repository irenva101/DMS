using DMS.Shared.Attributes;

namespace DMS.Shared.FileModels
{
    public class UserXls
    {
        [Name("First Name *")]
        public string FirstName { get; set; }

        [Name("Middle Name")]
        public string? MiddleName { get; set; }

        [Name("Last Name *")]
        public string LastName { get; set; }

        [Name("Email *")]
        public required string Email { get; set; }

        [Name("Role Name *")]
        public string RoleName { get; set; }

        [Name("Address")]
        public string? Address { get; set; }

        [Name("Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}
