using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkFlowNextStepConfiguration : IEntityTypeConfiguration<WorkFlowNextStep>
{
    public void Configure(EntityTypeBuilder<WorkFlowNextStep> builder)
    {
        builder.HasOne(n => n.NextStepFromWfd)
            .WithMany(t => t.WorkFlowNextStepsFrom)
            .HasForeignKey(d => d.FromWfdId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.NextStepToWfd)
            .WithMany(c => c.WorkFlowNextStepsTo)
            .HasForeignKey(s => s.ToWfdId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}