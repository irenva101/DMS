using DMS.Library.Repositories.Models;
using DMS.Shared.Helper;
using Microsoft.EntityFrameworkCore;

namespace DMS.Library.Data
{
    public class DmsContext : DbContext
    {
        public DmsContext(DbContextOptions<DmsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dms");
            modelBuilder.UseSerialColumns();


            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DmsContext).Assembly);

            // Register Enums in DB
            modelBuilder.RegisterEnums<DmsContext>();
            base.OnModelCreating(modelBuilder);
        }
        //public DbSet<ImportFileEntity> ImportFiles { get; set; }
        public DbSet<ProcessTypeEntity> ProcessTypes { get; set; }
        public DbSet<StepDefinitionEntity> StepDefinitions { get; set; }
        public DbSet<ProcessEntity> Processes { get; set; }
        public DbSet<StepEntity> Steps { get; set; }
        public DbSet<DocumentEntity> Documents { get; set; }
        public DbSet<AuditLogEntity> AuditLogs { get; set; }
        public DbSet<CompanyEntity> Companies { get; set; }
        public DbSet<DocumentRequirementEntity> DocumentRequirements { get; set; }
        public DbSet<ContractEntity> Contracts { get; set; }
        public DbSet<BillEntity> Bills { get; set; }
        public DbSet<FormTemplateEntity> FormTemplates { get; set; }
        public DbSet<CodeSequenceEntity> CodeSequences { get; set; }
    }
}
