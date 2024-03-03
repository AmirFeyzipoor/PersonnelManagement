using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelManagement.Entities.AuditLogs;

namespace PersonnelManagement.Infrastructure.Data.AuditLogs;

public class AuditLogEntityMap : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        
        builder.Property(_ => _.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(_ => _.UserId).IsRequired();
        builder.Property(_ => _.Action).IsRequired();
        builder.Property(_ => _.EntityName).IsRequired();
        builder.Property(_ => _.Timestamp).IsRequired();
        builder.Property(_ => _.Changes).IsRequired();
        builder.Property(_ => _.EntityPrimaryKey).IsRequired();
    }
}