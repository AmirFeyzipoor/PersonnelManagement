using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    private readonly IHttpContextAccessor _accessor;
    public DataContext(string connectionString, IHttpContextAccessor accessor)
        : this(new DbContextOptionsBuilder<DataContext>()
            .UseSqlServer(connectionString).Options, accessor)
    {
    }

    private DataContext(DbContextOptions<DataContext> options, IHttpContextAccessor accessor)
        : base(options)
    {
        _accessor = accessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var modifiedEntities = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        foreach (var auditLog in modifiedEntities.Select(modifiedEntity => new AuditLog
                 {
                     UserId = _accessor.HttpContext!.User.Claims
                         .FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier)?.Value!,
                     EntityName = modifiedEntity.Entity.GetType().Name,
                     Action = modifiedEntity.State.ToString(),
                     Timestamp = DateTime.UtcNow,
                     Changes = GetChanges(modifiedEntity)
                 }))
        {
            Set<AuditLog>().Add(auditLog);
        }

        return base.SaveChangesAsync(cancellationToken);
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
}