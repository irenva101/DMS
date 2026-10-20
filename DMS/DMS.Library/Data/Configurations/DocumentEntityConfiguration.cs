using DMS.Library.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.Library.Data.Configurations
{
    public class DocumentEntityConfiguration : IEntityTypeConfiguration<DocumentEntity>
    {
        public void Configure(EntityTypeBuilder<DocumentEntity> builder)
        {
            builder.ToTable(t =>
            {
                // At most one owner; a document without one is standalone in the archive
                t.HasCheckConstraint(
                    "CK_documents_single_owner",
                    "num_nonnulls(step_id, contract_id, bill_id, form_template_id) <= 1");

                t.HasCheckConstraint(
                    "CK_documents_requirement_needs_step",
                    "requirement_id IS NULL OR step_id IS NOT NULL");
            });
        }
    }
}
