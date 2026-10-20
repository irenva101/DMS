using DMS.Library.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.Library.Data.Configurations
{
    public class StepDefinitionEntityConfiguration : IEntityTypeConfiguration<StepDefinitionEntity>
    {
        // A hard delete of a template step must not take process steps or requirements with it
        public void Configure(EntityTypeBuilder<StepDefinitionEntity> builder)
        {
            builder.HasMany(x => x.DocumentRequirements)
                .WithOne(x => x.StepDefinition)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<StepEntity>()
                .WithOne(x => x.StepDefinition)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
