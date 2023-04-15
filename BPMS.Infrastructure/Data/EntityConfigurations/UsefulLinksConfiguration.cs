using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class UsefulLinksConfiguration : IEntityTypeConfiguration<UsefulLinks>
{
    public void Configure(EntityTypeBuilder<UsefulLinks> builder)
    {
        builder.HasKey(c => c.Id);
    }
}