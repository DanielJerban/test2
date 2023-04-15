using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class ServiceTaskLogConfiguration : IEntityTypeConfiguration<ServiceTaskLog>
{
    public void Configure(EntityTypeBuilder<ServiceTaskLog> builder)
    {
        builder.HasKey(c => c.Id);
    }
}