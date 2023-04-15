using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class ScheduleLogConfiguration : IEntityTypeConfiguration<ScheduleLog>
{
    public void Configure(EntityTypeBuilder<ScheduleLog> builder)
    {
        builder.HasKey(c => c.Id);
    }
}