using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class RoleMapChartConfiguration : IEntityTypeConfiguration<RoleMapChart>
{
    public void Configure(EntityTypeBuilder<RoleMapChart> builder)
    {
        builder.HasOne(ra => ra.Chart)
            .WithMany(u => u.RoleMapCharts)
            .HasForeignKey(ra => ra.ChartId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ra => ra.Role)
            .WithMany(r => r.RoleMapCharts)
            .HasForeignKey(ra => ra.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(c => new
        {
            c.RoleId,
            c.ChartId
        }, "IX_RoleId_ChartId").IsUnique();
    }
}