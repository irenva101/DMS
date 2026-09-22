using DLMS.Tenants.Data;
using DLMS.Tenants.Repositories.Models;
using DMS.Shared.Commons;
using Microsoft.EntityFrameworkCore;

namespace DLMS.Tenants.Constants
{
    public class PermissionSeeder(SharedContext _tenantContext)
    {
        private const string SuperAdminRoleName = "SuperAdmin";

        /// <summary>
        /// Seed permission
        /// </summary>
        /// <returns>void</returns>
        public async Task SeedPermissionAsync()
        {

            var utilities = await _tenantContext.Utilities
                .Where(r => !r.IsDeleted).ToListAsync();

            var superAdminRoles = await _tenantContext.Roles.Where(r => r.Name.ToLower() == SuperAdminRoleName.ToLower()).ToListAsync();
            foreach (var u in utilities)
            {
                var superAdminRole = superAdminRoles.FirstOrDefault(x => x.UtilityId == u.Id);
                if (superAdminRole == null)
                {
                    superAdminRole = new RoleEntity
                    {
                        Name = SuperAdminRoleName,
                        CreatedTime = DateTime.UtcNow,
                        DmsId = Guid.NewGuid(),
                        Status = DMS.Shared.Constants.UserStatus.Active,
                        UtilityId = u.Id
                    };
                    _tenantContext.Roles.Add(superAdminRole);
                    await _tenantContext.SaveChangesAsync();
                }
            }
            superAdminRoles = await _tenantContext.Roles.Where(r => r.Name.ToLower() == SuperAdminRoleName.ToLower()).ToListAsync();

            // All values from the enum
            List<PermissionsEnum> enumPermissions = Enum.GetValues<PermissionsEnum>().ToList();

            // Values from the database (assuming PermissionEntity.Value is of type PermissionsEnum or convertible)
            var dbValues = await _tenantContext.Permissions.ToListAsync();

            // Permissions that exist in the enum but are missing in the database
            var missingInDb = enumPermissions.Except(dbValues.Select(x => x.Value).ToList()).ToList();

            // Permissions that exist in the database but are missing in the enum
            var missingInEnum = dbValues.Select(x => x.Value).ToList().Except(enumPermissions).ToList();



            if (missingInDb != null && missingInDb.Count != 0)
            {

                foreach (var perm in missingInDb)
                {
                    var permission = new PermissionEntity
                    {
                        DmsId = Guid.NewGuid(),
                        LastUpdateTime = DateTime.UtcNow,
                        Value = perm,
                        Roles = superAdminRoles,
                    };
                    _tenantContext.Permissions.Add(permission);
                }
                await _tenantContext.SaveChangesAsync();
            }

            // Delete removed permissions
            if (missingInEnum.Any())
            {
                // Get the PermissionEntity objects from the database that should be removed
                var forRemove = dbValues
                    .Where(p => missingInEnum.Contains(p.Value)) // p.Value is PermissionsEnum
                    .ToList();

                // Remove them from the DbSet
                _tenantContext.Permissions.RemoveRange(forRemove);
                await _tenantContext.SaveChangesAsync();
            }

            // Delete removed permissions
            if (missingInEnum.Any())
            {
                // Get the PermissionEntity objects from the database that should be removed
                var forRemove = dbValues
                    .Where(p => missingInEnum.Contains(p.Value)) // p.Value is PermissionsEnum
                    .ToList();

                // Remove them from the DbSet
                _tenantContext.Permissions.RemoveRange(forRemove);
                await _tenantContext.SaveChangesAsync();
            }
            //all all permisions to superadmin roles
            var permissions = await _tenantContext.Permissions.Where(r => !r.IsDeleted).ToListAsync();
            superAdminRoles = await _tenantContext.Roles.Include(x => x.Permissions).Where(r => r.Name.ToLower() == SuperAdminRoleName.ToLower()).ToListAsync();
            foreach (var a in superAdminRoles)
            {
                a.Permissions = permissions;
            }
            await _tenantContext.SaveChangesAsync();

        }
    }
}
