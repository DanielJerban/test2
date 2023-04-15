using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class FormClassificationAccessConfiguration : IEntityTypeConfiguration<FormClassificationAccess>
{
    public void Configure(EntityTypeBuilder<FormClassificationAccess> builder)
    {
        builder.ToTable("FormClassificationAccesses");
    }
}