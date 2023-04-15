using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class WorkFlowFormListConfiguration : IEntityTypeConfiguration<WorkFlowFormList>
{
    public void Configure(EntityTypeBuilder<WorkFlowFormList> builder)
    {
        builder.ToTable("WorkFlowFormLists");
    }
}