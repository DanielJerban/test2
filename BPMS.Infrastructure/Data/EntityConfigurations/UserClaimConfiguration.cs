using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(ra => ra.User)
            .WithMany(u => u.UserClaims)
            .HasForeignKey(ra => ra.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}