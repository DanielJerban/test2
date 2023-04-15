using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class SmsLogConfiguration : IEntityTypeConfiguration<SmsLog>
{
    public void Configure(EntityTypeBuilder<SmsLog> builder)
    {
        builder.HasKey(c => c.Id);
    }
}