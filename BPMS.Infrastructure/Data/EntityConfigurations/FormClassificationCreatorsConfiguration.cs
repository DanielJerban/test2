using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class FormClassificationCreatorsConfiguration : IEntityTypeConfiguration<FormClassificationCreators>
{
    public void Configure(EntityTypeBuilder<FormClassificationCreators> builder)
    {
        builder.HasOne(r => r.CreatorType).WithMany(m => m.FormClassificationCreators_CreatorType);
        builder.HasOne(r => r.Staff).WithMany(m => m.FormClassificationCreators_Staff);
        builder.HasOne(r => r.FormClassification).WithMany(m => m.FormClassificationCreators);
    }
}