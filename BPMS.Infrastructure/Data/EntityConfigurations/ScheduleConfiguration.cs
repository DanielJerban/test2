using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasOne(s => s.TaskType)
            .WithMany(l => l.Schedules)
            .HasForeignKey(s => s.TaskTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}