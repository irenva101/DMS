using DMS.Library.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.Library.Data.Configurations
{
    public class CompanyEntityConfiguration : IEntityTypeConfiguration<CompanyEntity>
    {
        public void Configure(EntityTypeBuilder<CompanyEntity> builder)
        {
            // Unique only when filled in
            builder.HasIndex(x => x.TaxId)
                .IsUnique()
                .HasFilter("tax_id IS NOT NULL");

            // Records are soft-deleted; a hard delete must not silently take dependents with it
            builder.HasMany<ProcessEntity>()
                .WithOne(x => x.Company)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<ContractEntity>()
                .WithOne(x => x.Company)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<BillEntity>()
                .WithOne(x => x.Company)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
