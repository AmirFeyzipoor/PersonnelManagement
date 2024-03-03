using Microsoft.EntityFrameworkCore;
using PersonnelManagement.Entities.AuditLogs;
using PersonnelManagement.UseCases.Infrastructure.AuditLogs;

namespace PersonnelManagement.Infrastructure.Data.AuditLogs;

public class EfAuditLogRepository : IAuditLogRepository
{
    private readonly DbSet<AuditLog> _logs;

    public EfAuditLogRepository(DataContext context)
    {
        _logs = context.Set<AuditLog>();
    }

    public void AddLog(AuditLog log)
    {
        _logs.Add(log);
    }
}