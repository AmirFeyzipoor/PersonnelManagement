using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelManagement.Entities.AuditLogs;

namespace PersonnelManagement.Infrastructure.Data.AuditLogs;

public class AuditLogEntityMap : IEntityTypeConfiguration<UserAuditLog>
{
    public void Configure(EntityTypeBuilder<UserAuditLog> builder)
    {
        builder.ToTable("UserAuditLogs");
        
        builder.Property(_ => _.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(_ => _.UserId).IsRequired();
        builder.Property(_ => _.Action).IsRequired();
        builder.Property(_ => _.RegistrantId).IsRequired();
        builder.Property(_ => _.Timestamp).IsRequired();
        builder.Property(_ => _.Changes).IsRequired();

        builder.HasOne(_ => _.User)
            .WithMany(_ => _.Logs)
            .HasForeignKey(_ => _.UserId);
    }
}