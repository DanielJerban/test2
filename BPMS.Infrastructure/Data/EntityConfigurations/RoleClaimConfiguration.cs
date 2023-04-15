using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasOne(ra => ra.Role)
            .WithMany(u => u.RoleClaims)
            .HasForeignKey(ra => ra.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}