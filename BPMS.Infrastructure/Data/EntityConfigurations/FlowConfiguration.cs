using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class FlowConfiguration : IEntityTypeConfiguration<Flow>
{
    public void Configure(EntityTypeBuilder<Flow> builder)
    {
        builder.HasOne(c => c.PreviousFlow)
            .WithMany()
            .HasForeignKey(c => c.PreviousFlowId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.Staff)
            .WithMany(l => l.FlowStaff)
            .HasForeignKey(c => c.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.Request)
            .WithMany(l => l.Flows)
            .HasForeignKey(c => c.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.LookUpFlowStatus)
            .WithMany(l => l.FlowsStatus)
            .HasForeignKey(c => c.FlowStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.OrganizationPostTitle)
            .WithMany(l => l.FlowsOrganizationPostTitle)
            .HasForeignKey(c => c.OrganizationPostTitleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.WorkFlowDetail)
            .WithMany(l => l.Flows)
            .HasForeignKey(c => c.WorkFlowDetailId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.ConfermentAuthorityStaff)
            .WithMany(l => l.ConfermentAuthorityFlow)
            .HasForeignKey(c => c.ConfermentAuthorityStaffId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}