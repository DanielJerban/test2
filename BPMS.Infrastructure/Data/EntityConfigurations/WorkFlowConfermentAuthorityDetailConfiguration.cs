using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkFlowConfermentAuthorityDetailConfiguration : IEntityTypeConfiguration<WorkFlowConfermentAuthorityDetail>
{
    public void Configure(EntityTypeBuilder<WorkFlowConfermentAuthorityDetail> builder)
    {
        builder.HasOne(c => c.Staffs)
            .WithMany(l => l.WorkFlowConfermentAuthorityDetail)
            .HasForeignKey(c => c.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.WorkFlowConfermentAuthority)
            .WithMany(l => l.WorkFlowConfermentAuthorityDetail)
            .HasForeignKey(c => c.ConfermentAuthorityId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}