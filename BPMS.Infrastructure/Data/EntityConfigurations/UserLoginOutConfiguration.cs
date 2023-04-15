using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class UserLoginOutConfiguration : IEntityTypeConfiguration<UserLoginOut>
{
    public void Configure(EntityTypeBuilder<UserLoginOut> builder)
    {
        builder.HasOne(ulo => ulo.User)
            .WithMany(u => u.UserLoginOuts)
            .HasForeignKey(ulo => ulo.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ulo => ulo.UserLoginoutType)
            .WithMany(l => l.UserLoginOuts)
            .HasForeignKey(ulo => ulo.LoginOutTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}