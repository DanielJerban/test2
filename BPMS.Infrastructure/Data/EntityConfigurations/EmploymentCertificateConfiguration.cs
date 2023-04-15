using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class EmploymentCertificateConfiguration : IEntityTypeConfiguration<EmployementCertificate>
{
    public void Configure(EntityTypeBuilder<EmployementCertificate> builder)
    {
        builder.ToTable("EmployementCertificates");

        builder.HasOne(c => c.Requests)
            .WithMany(l => l.EmpCertificates)
            .HasForeignKey(c => c.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}