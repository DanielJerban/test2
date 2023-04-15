using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkFlowDetailConfiguration : IEntityTypeConfiguration<WorkFlowDetail>
{
    public void Configure(EntityTypeBuilder<WorkFlowDetail> builder)
    {
        builder.HasOne(l => l.WorkFlow)
            .WithMany(u => u.WorkflowDetails)
            .HasForeignKey(e => e.WorkFlowId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(k => k.Staff)
            .WithMany(z => z.WorkflowDetails)
            .HasForeignKey(q => q.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(a => a.OrganizationPostTitle)
            .WithMany(f => f.WorkFlowDetailOrganizationPostTitles)
            .HasForeignKey(t => t.OrganizationPostTitleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.OrganizationPostType)
            .WithMany(o => o.WorkFlowDetailOrganizationPostTypes)
            .HasForeignKey(r => r.OrganizationPostTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.ResponseGroup)
            .WithMany(o => o.WorkFlowDetailResponseGroups)
            .HasForeignKey(r => r.ResponseGroupId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.WorkFlowForm)
            .WithMany(d => d.WorkFlowDetails)
            .HasForeignKey(w => w.WorkFlowFormId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.ExternalApi)
            .WithMany(c => c.WorkFlowDetails)
            .HasForeignKey(c => c.ExternalApiId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}