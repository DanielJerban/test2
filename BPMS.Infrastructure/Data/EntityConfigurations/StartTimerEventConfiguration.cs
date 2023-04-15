using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class StartTimerEventConfiguration : IEntityTypeConfiguration<StartTimerEvent>
{
    public void Configure(EntityTypeBuilder<StartTimerEvent> builder)
    {
        builder.HasOne(a => a.Workflow)
            .WithOne(ad => ad.StartTimerEvent)
            .OnDelete(DeleteBehavior.Cascade);
    }
}