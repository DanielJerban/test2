using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class OrganizationInfoConfiguration : IEntityTypeConfiguration<OrganiztionInfo>
{
    public void Configure(EntityTypeBuilder<OrganiztionInfo> builder)
    {
        builder.ToTable("OrganiztionInfoes");

        builder.HasOne(o => o.Staff)
            .WithMany(s => s.OrganiztionInfos)
            .HasForeignKey(o => o.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Chart)
            .WithMany(c => c.OrganiztionInfos)
            .HasForeignKey(o => o.ChartId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.OrganiztionPost)
            .WithMany(l => l.OrganiztionInfos)
            .HasForeignKey(o => o.OrganiztionPostId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}