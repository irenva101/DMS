using DLMS.Tenants.Data;
using DLMS.Tenants.Repositories.Models;
using DMS.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace DLMS.Tenants.Constants
{
    public class TenantSeeder(SharedContext _tenantContext)
    {
        private const string DefaultUtilityAcronym = "DEFAULT";
        private const string DefaultUserEmail = "admin@dms.local";
        private const string SuperAdminRoleName = "SuperAdmin";

        /// <summary>
        /// Ensures a default tenant (utility) and a default admin user exist.
        /// </summary>
        public async Task SeedDefaultTenantAsync()
        {
            var utility = await _tenantContext.Utilities
                .FirstOrDefaultAsync(u => u.Acronym == DefaultUtilityAcronym);

            if (utility == null)
            {
                utility = new UtilityEntity
                {
                    Name = "Default Utility",
                    Acronym = DefaultUtilityAcronym,
                    ApiKey = Guid.NewGuid(),
                    Status = UtilityStatus.Active,
                    Address = string.Empty,
                    WebsiteAddress = string.Empty,
                    TimeZone = 0,
                    CreationDate = DateTime.UtcNow
                };
                _tenantContext.Utilities.Add(utility);
                await _tenantContext.SaveChangesAsync();
            }

            var user = await _tenantContext.Users
                .FirstOrDefaultAsync(u => u.Email == DefaultUserEmail);

            if (user == null)
            {
                user = new UserEntity
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = DefaultUserEmail,
                    Status = UserStatus.Active,
                    CreatedTime = DateTime.UtcNow,
                    UtilityId = utility.Id,
                    isAdmin = true
                };
                _tenantContext.Users.Add(user);
                await _tenantContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Links the default user to the SuperAdmin role. Must run after PermissionSeeder,
        /// since that's what creates the SuperAdmin role for the default tenant.
        /// </summary>
        public async Task AssignSuperAdminRoleToDefaultUserAsync()
        {
            var user = await _tenantContext.Users
                .FirstOrDefaultAsync(u => u.Email == DefaultUserEmail);

            if (user == null || user.RoleId != null)
            {
                return;
            }

            var role = await _tenantContext.Roles
                .FirstOrDefaultAsync(r => r.Name == SuperAdminRoleName && r.UtilityId == user.UtilityId);

            if (role == null)
            {
                return;
            }

            user.RoleId = role.Id;
            await _tenantContext.SaveChangesAsync();
        }
    }
}
