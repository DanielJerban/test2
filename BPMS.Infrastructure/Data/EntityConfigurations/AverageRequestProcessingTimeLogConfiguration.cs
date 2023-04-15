using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class AverageRequestProcessingTimeLogConfiguration : IEntityTypeConfiguration<AverageRequestProcessingTimeLog>
{
    public void Configure(EntityTypeBuilder<AverageRequestProcessingTimeLog> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.WorkFlowId).IsUnique();
    }
}