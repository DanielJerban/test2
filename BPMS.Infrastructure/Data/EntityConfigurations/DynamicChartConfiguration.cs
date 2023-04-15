using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class DynamicChartConfiguration : IEntityTypeConfiguration<DynamicChart>
{
    public void Configure(EntityTypeBuilder<DynamicChart> builder)
    {
        builder.HasOne(c => c.Report)
            .WithMany(l => l.DynamicCharts)
            .HasForeignKey(c => c.ReportId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.WidgetType)
            .WithMany(l => l.DynamicChartsWidgetType)
            .HasForeignKey(c => c.WidgetTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.Creator)
            .WithMany(l => l.DynamicCharts)
            .HasForeignKey(c => c.CreatorId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}