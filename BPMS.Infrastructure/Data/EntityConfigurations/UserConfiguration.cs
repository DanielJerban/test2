using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasOne(u => u.Staff)
            .WithMany(s => s.Users)
            .HasForeignKey(u => u.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(c => c.SystemSettings)
            .WithOne(c => c.CreatorUser)
            .HasForeignKey(c => c.CreatorUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(c => c.StaffId).IsUnique();
        builder.HasIndex(c => c.UserName).IsUnique();
    }
}