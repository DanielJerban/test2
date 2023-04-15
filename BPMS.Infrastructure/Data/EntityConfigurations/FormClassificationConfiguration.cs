using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class FormClassificationConfiguration : IEntityTypeConfiguration<FormClassification>
{
    public void Configure(EntityTypeBuilder<FormClassification> builder)
    {
        builder.HasOne(r => r.FormType)
            .WithMany(m => m.FormClassification_FormType)
            .HasForeignKey(f => f.FormTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.FormStatus)
            .WithMany(m => m.FormClassification_FormStatus)
            .HasForeignKey(f => f.FormStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.StandardType)
            .WithMany(m => m.FormClassification_StandardType)
            .HasForeignKey(f => f.StandardTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.AccessType)
            .WithMany(m => m.FormClassification_AccessType)
            .HasForeignKey(f => f.AccessTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.WorkFlowLookup)
            .WithMany(c => c.FormClassification_WorkFlowLookup)
            .HasForeignKey(c => c.WorkFlowLookupId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.ConfidentialLevel)
            .WithMany(c => c.FormClassification_ConfidentialLevel)
            .HasForeignKey(c => c.ConfidentialLevelId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}