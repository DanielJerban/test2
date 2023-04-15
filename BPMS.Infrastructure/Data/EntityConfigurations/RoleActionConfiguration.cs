using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BPMS.Infrastructure.Data.EntityConfigurations;

public class RoleActionConfiguration : IEntityTypeConfiguration<RoleAction>
{
    public void Configure(EntityTypeBuilder<RoleAction> builder)
    {
        builder.HasIndex(c => new
        {
            c.Controller,
            c.Action,
            c.RoleId
        }, "IX_Controller_Action_RoleId").IsUnique();
    }
}