using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(w => w.Workflow)
            .WithMany(l => l.Requests)
            .HasForeignKey(w => w.WorkFlowId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.RequestStatus)
            .WithMany(l => l.RequestStatuses)
            .HasForeignKey(w => w.RequestStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.Staff)
            .WithMany(l => l.Requests)
            .HasForeignKey(w => w.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.OrganizationPostTitle)
            .WithMany(l => l.RequestPostTitles)
            .HasForeignKey(w => w.OrganizationPostTitleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}