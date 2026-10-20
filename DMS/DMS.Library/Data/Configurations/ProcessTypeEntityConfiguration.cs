using DMS.Library.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.Library.Data.Configurations
{
    public class ProcessTypeEntityConfiguration : IEntityTypeConfiguration<ProcessTypeEntity>
    {
        // Templates are deactivated, not deleted; a hard delete must not take processes with it
        public void Configure(EntityTypeBuilder<ProcessTypeEntity> builder)
        {
            builder.HasMany(x => x.StepDefinitions)
                .WithOne(x => x.ProcessType)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<ProcessEntity>()
                .WithOne(x => x.ProcessType)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
