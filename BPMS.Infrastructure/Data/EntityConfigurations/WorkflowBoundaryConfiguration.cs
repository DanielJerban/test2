using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkflowBoundaryConfiguration : IEntityTypeConfiguration<WorkFlowBoundary>
{
    public void Configure(EntityTypeBuilder<WorkFlowBoundary> builder)
    {
        builder.HasOne(d => d.WorkFlowDetail)
            .WithMany(d => d.WorkFlowBoundaries)
            .HasForeignKey(d => d.WorkflowDetailId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}