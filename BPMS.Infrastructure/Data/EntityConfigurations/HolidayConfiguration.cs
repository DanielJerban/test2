using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class HolidayConfiguration : IEntityTypeConfiguration<Holyday>
{
    public void Configure(EntityTypeBuilder<Holyday> builder)
    {
        builder.HasOne(d => d.HolydayType)
            .WithMany(d => d.HolydayTypes)
            .HasForeignKey(d => d.HolydayTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}