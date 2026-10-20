using DLMS.Tenants;
using DMS.Library;
using DMS.Shared.Constants;

namespace DMS.API.Helper
{
    // Runs migrations in the background so a long migration does not block startup (LongMigration = true)
    public class ExecuteMigrationsDms : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExecuteMigrationsDms> _logger;

        public ExecuteMigrationsDms(IServiceScopeFactory scopeFactory, ILogger<ExecuteMigrationsDms> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = Task.Run(() => ExecuteAll(), cancellationToken);
            return Task.CompletedTask;
        }

        private async Task ExecuteAll()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                var isTest = env.IsEnvironment(nameof(AppSettingsEnvironment.Test));

                scope.ExecutePendingTenantMigrations();

                var results = await scope.ExecutePendingDmsMigrations(_logger, isTest);

                var failed = results.Where(r => !r.Success).ToList();
                foreach (var r in failed)
                    _logger.LogError("Migration {Context} for {Acronym} failed: {Error}", r.Context, r.Acronym, r.Error);

                _logger.LogInformation("Migrations finished in the hosted service. {Success} succeeded, {Failed} failed.", results.Count - failed.Count, failed.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue with executing migrations in the hosted service.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
