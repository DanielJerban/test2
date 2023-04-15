using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class LookUpConfiguration : IEntityTypeConfiguration<LookUp>
{
    public void Configure(EntityTypeBuilder<LookUp> builder)
    {
        builder.HasIndex(c => new
        {
            c.Code,
            c.Type
        }, "XI_Code_Type").IsUnique();
    }
}