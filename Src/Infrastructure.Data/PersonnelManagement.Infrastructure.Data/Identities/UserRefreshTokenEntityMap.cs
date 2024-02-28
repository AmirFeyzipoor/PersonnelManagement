using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelManagement.Entities.Identities;

namespace PersonnelManagement.Infrastructure.Data.Identities;

public class UserRefreshTokenEntityMap : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.ToTable("UserRefreshTokens");
        
        builder.HasKey(_ => _.UserId);

        builder.Property(_ => _.RefreshTokenExpiryTime).IsRequired();
        builder.Property(_ => _.Token).IsRequired();
        
        builder.HasOne(_ => _.User)
            .WithOne(_ => _.RefreshToken)
            .HasForeignKey<UserRefreshToken>(_ => _.UserId);
    }
}