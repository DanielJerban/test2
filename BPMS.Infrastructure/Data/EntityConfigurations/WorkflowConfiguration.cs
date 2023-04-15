using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.HasOne(w => w.RequestType)
            .WithMany(l => l.WorkflowsRequestType)
            .HasForeignKey(w => w.RequestTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.FlowType)
            .WithMany(l => l.WorkflowsFlowType)
            .HasForeignKey(w => w.FlowTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.RequestGroupType)
            .WithMany(l => l.WorkflowsRequestGroupType)
            .HasForeignKey(w => w.RequestGroupTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.Staff)
            .WithMany(o => o.Workflows)
            .HasForeignKey(c => c.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.Modifier)
            .WithMany(x => x.WorkflowsModifier)
            .HasForeignKey(s => s.ModifiedId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.SubProcess)
            .WithMany(m => m.SubProcesses)
            .HasForeignKey(f => f.SubProcessId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}