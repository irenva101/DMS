using DLMS.Tenants.Services.Interfaces;
using DLMS.Tenants.Services.Models.Utility.DataOut;
using DMS.Library.Data;
using DMS.Library.Repositories.UOWs;
using DMS.Shared.Config;
using DMS.Shared.DTOs;
using DMS.Shared.Handlers.Model;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Text.Json;

namespace DMS.Library
{
    public static class DmsSetup
    {
        public static IServiceCollection AddDmsDatabase(
            this IServiceCollection collection,
            string connectionString
        )
        {
            collection.AddDbContext<DmsContext>((sp, options) =>
            {
                var userContext = sp.GetRequiredService<UserContext>();
                var userOpsConfig = sp.GetRequiredService<DmsConfig>();

                string connS = connectionString;

                if (!string.IsNullOrEmpty(userContext.UtilityName))
                {
                    string prefix = userContext.UtilityName;
                    connS = userOpsConfig.ConnectionStrings.DmsDb;
                    connS = connS.Replace("{db_name}", prefix.ToLower());
                }

                options
                    .UseNpgsql(
                        connS,
                        sqlServerOptions =>
                        {
                            sqlServerOptions.EnableRetryOnFailure(
                                errorCodesToAdd: [PostgresErrorCodes.LockNotAvailable]);

                            sqlServerOptions.CommandTimeout(46000);
                            sqlServerOptions.MigrationsHistoryTable("_dmsContextMigrationsHistory", "dms");

                            sqlServerOptions.ConfigureDataSource(dataSourceBuilder =>
                                dataSourceBuilder.EnableDynamicJson()
                                    .ConfigureJsonOptions(new JsonSerializerOptions
                                    {
                                        AllowOutOfOrderMetadataProperties = true
                                    }));
                        }
                    )
                    .EnableSensitiveDataLogging();
            });
            return collection;
        }

        public static IServiceCollection AddDmsServices(this IServiceCollection collection, DmsConfig appConfig)
        {
            collection.AddScoped<IUnitOfWork, UnitOfWork>();
            //collection.AddScoped<IFileService, FileService>();
            //if (appConfig.Sftp.SftpOnPremise)
            //{
            //    collection.AddScoped<IUploadService, SftpUploadService>();
            //    collection.AddScoped<IBinaryStorageService, SftpBinaryStorageService>();
            //}
            //else
            //{
            //    collection.AddSingleton(x => new BlobServiceClient(appConfig.Sftp.BlobConnectionString));
            //    collection.AddScoped<IUploadService, BlobUploadService>();
            //    collection.AddScoped<IBinaryStorageService, BlobBinaryStorageService>();
            //}


            // Register all validators from assembly using FluentValidation's built-in extension
            collection.AddValidatorsFromAssembly(typeof(DmsSetup).Assembly);

            collection.AddHttpContextAccessor();
            return collection;
        }
        public async static Task<List<MigrationResult>> ExecutePendingDmsMigrations(
            this IServiceScope scope,
            ILogger logger,
            bool test = false)
        {
            var results = new List<MigrationResult>();

            logger.LogInformation("Update database for all utilities started");
            try
            {
                var services = scope.ServiceProvider;
                var utilityService = services.GetRequiredService<IUtilityService>();

                logger.LogInformation("Get all utilities from home service");

                var utilities = await utilityService.GetAll();

                if (utilities.IsFailure)
                {
                    logger.LogError("Failed to retrieve utilities: {Error}", utilities.Error);
                    results.Add(new MigrationResult { Context = "Dms", Acronym = "(all)", Success = false, Error = utilities.Error.Message });
                    return results;
                }

                logger.LogInformation("Found {Count} utilities", utilities.Value.Count);

                if (test)
                {
                    utilities.Value.Add(new UtilityDto()
                    {
                        Acronym = "test"
                    });
                }

                var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();

                foreach (var utility in utilities.Value)
                {
                    try
                    {
                        await using var utilityScope = scopeFactory.CreateAsyncScope();

                        var userContext = utilityScope.ServiceProvider.GetRequiredService<UserContext>();
                        userContext.UtilityName = utility.Acronym;

                        logger.LogInformation("Database migrate for {Acronym} started", utility.Acronym);

                        var context = utilityScope.ServiceProvider.GetRequiredService<DmsContext>();

                        await context.Database.MigrateAsync();

                        logger.LogInformation("Database migrate for {Acronym} finished", utility.Acronym);
                        results.Add(new MigrationResult { Context = "Dms", Acronym = utility.Acronym, Success = true });
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Database migrate for {Acronym} failed. Exception: {Message}", utility.Acronym, ex.Message);
                        results.Add(new MigrationResult { Context = "Dms", Acronym = utility.Acronym, Success = false, Error = ex.Message });
                    }
                }

                logger.LogInformation("Update database for all utilities finished");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating the Dms database: {Message}", ex.Message);
                results.Add(new MigrationResult { Context = "Dms", Acronym = "(all)", Success = false, Error = ex.Message });
            }

            return results;
        }

    }
}
