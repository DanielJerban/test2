using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkflowDetailPatternConfiguration : IEntityTypeConfiguration<WorkflowDetailPattern>
{
    public void Configure(EntityTypeBuilder<WorkflowDetailPattern> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasMany(c => c.WorkflowPatternItems)
            .WithOne(c => c.WorkflowDetailPattern)
            .HasForeignKey(c => c.WorkflowDetailPatternId).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.WorkFlowDetails)
            .WithOne(c => c.WorkflowDetailPattern)
            .HasForeignKey(c => c.WorkflowDetailPatternId);
    }
}