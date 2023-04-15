using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class RoleMapPostTypeConfiguration : IEntityTypeConfiguration<RoleMapPostType>
{
    public void Configure(EntityTypeBuilder<RoleMapPostType> builder)
    {
        builder.HasOne(ra => ra.PostType)
            .WithMany(u => u.RoleMapPostTypes)
            .HasForeignKey(ra => ra.PostTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ra => ra.Role)
            .WithMany(r => r.RoleMapPostTypes)
            .HasForeignKey(ra => ra.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(c => new
        {
            c.RoleId,
            c.PostTypeId
        }, "IX_RoleId_LookupId").IsUnique();
    }
}