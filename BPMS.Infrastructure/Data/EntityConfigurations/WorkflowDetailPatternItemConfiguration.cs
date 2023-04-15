using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkflowDetailPatternItemConfiguration : IEntityTypeConfiguration<WorkflowDetailPatternItem>
{
    public void Configure(EntityTypeBuilder<WorkflowDetailPatternItem> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.LookUpOrganizationPost)
            .WithMany(c => c.WorkflowDetailPatternItems)
            .HasForeignKey(c => c.LookupOrganizationPostId);
    }
}