using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class RoleMapPostTitleConfiguration : IEntityTypeConfiguration<RoleMapPostTitle>
{
    public void Configure(EntityTypeBuilder<RoleMapPostTitle> builder)
    {
        builder.HasOne(ra => ra.PostTitle)
            .WithMany(u => u.RoleMapPostTitles)
            .HasForeignKey(ra => ra.PostTitleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ra => ra.Role)
            .WithMany(r => r.RoleMapPostTitles)
            .HasForeignKey(ra => ra.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(c => new
        {
            c.RoleId,
            c.PostTitleId
        }, "IX_RoleId_LookupId").IsUnique();
    }
}