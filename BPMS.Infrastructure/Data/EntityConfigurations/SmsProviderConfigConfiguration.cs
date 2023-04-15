using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class SmsProviderConfigConfiguration : IEntityTypeConfiguration<SmsProviderConfige>
{
    public void Configure(EntityTypeBuilder<SmsProviderConfige> builder)
    {
        builder.HasKey(c => c.Id);
    }
}