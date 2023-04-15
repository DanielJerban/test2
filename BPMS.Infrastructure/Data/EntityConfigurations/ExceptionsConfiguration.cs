using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class ExceptionsConfiguration : IEntityTypeConfiguration<Exceptions>
{
    public void Configure(EntityTypeBuilder<Exceptions> builder)
    {
        builder.ToTable("Exceptions");
    }
}