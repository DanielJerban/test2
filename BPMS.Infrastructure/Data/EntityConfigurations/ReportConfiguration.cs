using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasOne(w => w.Creator)
            .WithMany(l => l.Reports)
            .HasForeignKey(w => w.CreatorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.Workflow)
            .WithMany(l => l.Reports)
            .HasForeignKey(w => w.WorkflowId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}