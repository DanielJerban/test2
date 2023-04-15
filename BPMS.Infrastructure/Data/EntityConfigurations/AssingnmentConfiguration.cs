using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class AssingnmentConfiguration : IEntityTypeConfiguration<Assingnment>
{
    public void Configure(EntityTypeBuilder<Assingnment> builder)
    {
        builder.HasOne(c => c.Staff)
            .WithMany(l => l.Assingnments)
            .HasForeignKey(c => c.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.ResponseTypeGroup)
            .WithMany(l => l.Assingnments)
            .HasForeignKey(c => c.ResponseTypeGroupId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}