using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PersonnelManagement.Entities.AuditLogs;
using PersonnelManagement.Entities.Identities;

namespace PersonnelManagement.Infrastructure.Data;

public class DataContext : IdentityDbContext<
    User,
    Role,
    string,
    IdentityUserClaim<string>,
    UserRole,
    IdentityUserLogin<string>,
    IdentityRoleClaim<string>,
    IdentityUserToken<string>>
{
    public DataContext(string connectionString)
        : this(new DbContextOptionsBuilder<DataContext>()
            .UseSqlServer(connectionString).Options)
    {
    }

    private DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }

    public Task<int> SaveChangesAsync(string userId)
    {
        var modifiedEntities = ChangeTracker.Entries()
            .Where(e =>
                e.GetType() == typeof(User) &&
                e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        foreach (var auditLog in modifiedEntities.Select(modifiedEntity =>
                     new UserAuditLog
                     {
                         RegistrantId = userId,
                         UserId = modifiedEntity.OriginalValues["Id"]!.ToString()!,
                         Action = modifiedEntity.State.ToString(),
                         Timestamp = DateTime.UtcNow,
                         Changes = GetChanges(modifiedEntity)
                     }))
        {
            Set<UserAuditLog>().Add(auditLog);
        }

        return base.SaveChangesAsync();
    }

    private static string GetChanges(EntityEntry entity)
    {
        var changes = new StringBuilder();

        foreach (var property in entity.OriginalValues.Properties)
        {
            var originalValue = entity.OriginalValues[property];
            var currentValue = entity.CurrentValues[property];

            if (!Equals(originalValue, currentValue))
            {
                changes.AppendLine($"{property.Name}: From '{originalValue}' to '{currentValue}'");
            }
        }

        return changes.ToString();
    }

    private static string GetEntityPrimaryKey(EntityEntry entity)
    {
        return (from property in entity.Properties
                where property.Metadata.IsPrimaryKey()
                select property.CurrentValue.ToString())
            .First()!;
    }
}