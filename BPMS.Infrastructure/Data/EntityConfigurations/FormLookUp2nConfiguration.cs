using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class FormLookUp2NConfiguration : IEntityTypeConfiguration<FormLookUp2N>
{
    public void Configure(EntityTypeBuilder<FormLookUp2N> builder)
    {
    }
}