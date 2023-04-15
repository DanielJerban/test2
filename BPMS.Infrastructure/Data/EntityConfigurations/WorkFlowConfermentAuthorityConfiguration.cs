using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkFlowConfermentAuthorityConfiguration : IEntityTypeConfiguration<WorkFlowConfermentAuthority>
{
    public void Configure(EntityTypeBuilder<WorkFlowConfermentAuthority> builder)
    {
        builder.ToTable("WorkFlowConfermentAuthorities");

        builder.HasOne(c => c.Staff)
            .WithMany(l => l.WorkFlowConfermentAuthority)
            .HasForeignKey(c => c.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.LookUpRequestType)
            .WithMany(l => l.WorkFlowConfermentAuthorities)
            .HasForeignKey(c => c.RequestTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}