using Microsoft.EntityFrameworkCore;
using PersonnelManagement.Entities.AuditLogs;
using PersonnelManagement.UseCases.Infrastructure.AuditLogs;

namespace PersonnelManagement.Infrastructure.Data.AuditLogs;

public class EfAuditLogRepository : IAuditLogRepository
{
    private readonly DbSet<UserAuditLog> _logs;

    public EfAuditLogRepository(DataContext context)
    {
        _logs = context.Set<UserAuditLog>();
    }

    public void AddLog(UserAuditLog log)
    {
        _logs.Add(log);
    }
}