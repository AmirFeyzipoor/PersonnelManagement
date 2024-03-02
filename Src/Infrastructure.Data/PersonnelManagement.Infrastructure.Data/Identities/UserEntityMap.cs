using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelManagement.Entities.Identities;

namespace PersonnelManagement.Infrastructure.Data.Identities;

public class UserEntityMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(_ => _.Name)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(_ => _.LastName)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(_ => _.PhoneNumber).IsRequired(false);
        
        builder.Property(_ => _.CreationDate).IsRequired();
        
        builder.Property(_ => _.Email)
            .IsRequired(false);
        
        builder.Property(p => p.ConcurrencyStamp).IsConcurrencyToken();
    }
}