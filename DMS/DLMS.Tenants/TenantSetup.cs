using DLMS.Tenants.Constants;
using DLMS.Tenants.Data;
using DLMS.Tenants.Middleware;
using DLMS.Tenants.Repositories.UOWs;
using DMS.Shared.Handlers.Implementations;
using DMS.Shared.Handlers.Interfaces;
using DMS.Shared.Handlers.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DLMS.Tenants
{
    public static class TenantSetup
    {
        public static IServiceCollection AddSharedDatabase(
            this IServiceCollection collection,
            string connectionString
        )
        {
            collection.AddDbContextPool<SharedContext>(options =>
                options
                    .UseNpgsql(
                        connectionString,
                        sqlServerOptions =>
                        {
                            sqlServerOptions.EnableRetryOnFailure(
                                5,
                                TimeSpan.FromSeconds(20),
                                null
                            );
                            sqlServerOptions.CommandTimeout(46000);
                            sqlServerOptions.MigrationsHistoryTable(
                                "_tenantContextMigrationsHistory", "tenant"
                            );
                        }
                    )
                    //.UseOpenIddict()
                    .EnableSensitiveDataLogging()
            );
            return collection;
        }

        public static IServiceCollection AddTenantServices(this IServiceCollection collection)
        {
            collection.AddMemoryCache();

            //unit of work
            collection.AddScoped<IUnitOfWorkTenant, UnitOfWorkTenant>();

            //handlers
            collection.AddScoped<UserContext>();
            collection.AddScoped<IUserContextHandler, UserContextHandler>();

            //services
            //collection.AddScoped<IUtilityService, UtilityService>();
            //collection.AddScoped<IRoleService, RoleService>();
            //collection.AddScoped<IUserService, UserService>();
            //collection.AddScoped<IPermissionsService, PermissionsService>();
            //collection.AddScoped<IEmailService, EmailService>();

            collection.AddTransient<PermissionSeeder>();
            collection.AddTransient<TenantSeeder>();

            return collection;
        }
        public static IApplicationBuilder RegisterTenantMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<UserContextMiddleware>();
            return app;
        }

        public static void ExecutePendingTenantMigrations(this IServiceScope scope)
        {
            var db = scope.ServiceProvider.GetService<SharedContext>();
            var migrations = db.Database.GetPendingMigrations();
            if (migrations != null && migrations.Any())
            {
                db.Database.Migrate();
            }

            var tenantSeeder = scope.ServiceProvider.GetRequiredService<TenantSeeder>();
            tenantSeeder.SeedDefaultTenantAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            var permissionSeeder = scope.ServiceProvider.GetRequiredService<PermissionSeeder>();
            permissionSeeder.SeedPermissionAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            tenantSeeder.AssignSuperAdminRoleToDefaultUserAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
