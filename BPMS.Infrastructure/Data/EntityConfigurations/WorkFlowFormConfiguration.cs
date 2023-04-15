using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkFlowFormConfiguration : IEntityTypeConfiguration<WorkFlowForm>
{
    public void Configure(EntityTypeBuilder<WorkFlowForm> builder)
    {
        builder.HasOne(w => w.Staff)
            .WithMany(x => x.WorkFlowForms)
            .HasForeignKey(s => s.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.Modifier)
            .WithMany(x => x.WorkFlowFormsModifire)
            .HasForeignKey(s => s.ModifiedId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}