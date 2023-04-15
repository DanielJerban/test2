using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class ChartConfiguration : IEntityTypeConfiguration<Chart>
{
    public void Configure(EntityTypeBuilder<Chart> builder)
    {
        builder.HasOne(c => c.ChartLevel)
            .WithMany(l => l.Charts)
            .HasForeignKey(c => c.ChartLevelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.ChartParent)
            .WithMany(l => l.ChartChild)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}