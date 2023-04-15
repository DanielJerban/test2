using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class ExternalApiConfiguration : IEntityTypeConfiguration<ExternalApi>
{
    public void Configure(EntityTypeBuilder<ExternalApi> builder)
    {
    }
}