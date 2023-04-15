using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class EmailSettingsConfiguration : IEntityTypeConfiguration<EmailConfigs>
{
    public void Configure(EntityTypeBuilder<EmailConfigs> builder)
    {
        builder.HasKey(c => c.Id);
        builder.ToTable("EmailConfigs");
    }
}