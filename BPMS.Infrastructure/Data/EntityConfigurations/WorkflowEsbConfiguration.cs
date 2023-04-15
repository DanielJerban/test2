using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkflowEsbConfiguration : IEntityTypeConfiguration<WorkflowEsb>
{
    public void Configure(EntityTypeBuilder<WorkflowEsb> builder)
    {
        builder.HasOne(w => w.WorkFlowNextStep)
            .WithMany(x => x.WorkflowEsbs)
            .HasForeignKey(s => s.WorkflowNextStepId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}