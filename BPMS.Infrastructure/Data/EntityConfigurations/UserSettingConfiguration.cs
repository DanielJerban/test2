using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
{
    public void Configure(EntityTypeBuilder<UserSetting> builder)
    {
        builder.HasOne(c => c.User)
            .WithMany(l => l.UserSettings)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.SettingType)
            .WithMany(l => l.UserSettings)
            .HasForeignKey(c => c.SettingTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(c => new
        {
            c.UserId,
            c.SettingTypeId
        }, "XI_User_SettingType").IsUnique();
    }
}