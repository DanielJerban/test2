using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class RoleAccessConfiguration : IEntityTypeConfiguration<RoleAccess>
{
    public void Configure(EntityTypeBuilder<RoleAccess> builder)
    {
        builder.HasOne(ra => ra.User)
            .WithMany(u => u.RoleAccesses)
            .HasForeignKey(ra => ra.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ra => ra.Role)
            .WithMany(r => r.RoleAccesses)
            .HasForeignKey(ra => ra.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(c => new
        {
            c.RoleId,
            c.UserId
        }, "IX_RoleId_UserId").IsUnique();
    }
}