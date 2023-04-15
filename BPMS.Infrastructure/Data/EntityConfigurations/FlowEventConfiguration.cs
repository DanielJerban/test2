using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class FlowEventConfiguration : IEntityTypeConfiguration<FlowEvent>
{
    public void Configure(EntityTypeBuilder<FlowEvent> builder)
    {
        builder.HasOne(d => d.Flow)
            .WithMany(d => d.FlowEvents)
            .HasForeignKey(f => f.FlowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.WorkflowEsb)
            .WithMany(d => d.FlowEvents)
            .HasForeignKey(f => f.WorkFlowEsbId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}