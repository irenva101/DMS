using DMS.Library.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.Library.Data.Configurations
{
    public class AuditLogEntityConfiguration : IEntityTypeConfiguration<AuditLogEntity>
    {
        // Audit history must survive even if a referenced row is ever hard-deleted
        public void Configure(EntityTypeBuilder<AuditLogEntity> builder)
        {
            builder.HasOne(x => x.Process)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Step)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Document)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Contract)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Bill)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ProcessId, x.Time });
        }
    }
}
