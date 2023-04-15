using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkFlowIndicatorConfiguration : IEntityTypeConfiguration<WorkFlowIndicator>
{
    public void Configure(EntityTypeBuilder<WorkFlowIndicator> builder)
    {
        builder.HasOne(o => o.RequestType)
            .WithMany(l => l.WorkFlowIndicatorRequestType)
            .HasForeignKey(o => o.RequestTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.WorkFlowDetail)
            .WithMany(l => l.WorkFlowIndicators)
            .HasForeignKey(o => o.ActivityId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Duration)
            .WithMany(l => l.WorkFlowIndicatorDuration)
            .HasForeignKey(o => o.DurationId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Flowstatus)
            .WithMany(l => l.WorkFlowIndicatorFlowstatus)
            .HasForeignKey(o => o.FlowstatusId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.CalcCriterion)
            .WithMany(l => l.WorkFlowIndicatorCalcCriterion)
            .HasForeignKey(o => o.CalcCriterionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.WidgetType)
            .WithMany(l => l.WorkFlowIndicatorWidgetType)
            .HasForeignKey(o => o.WidgetTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}