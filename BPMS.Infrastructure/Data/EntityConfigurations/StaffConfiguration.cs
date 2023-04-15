using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.HasOne(w => w.StaffType)
            .WithMany(l => l.StaffType)
            .HasForeignKey(w => w.StaffTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.EngType)
            .WithMany(l => l.EngType)
            .HasForeignKey(w => w.EngTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(b => b.Building)
            .WithMany(l => l.Buildings)
            .HasForeignKey(b => b.BuildingId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(c => c.EngTypeId).IsRequired();
        builder.Property(c => c.StaffTypeId).IsRequired();
        builder.HasIndex(c => c.PersonalCode).IsUnique();
    }
}