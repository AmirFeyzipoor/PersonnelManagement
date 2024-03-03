using PersonnelManagement.Entities.AuditLogs;

namespace PersonnelManagement.UseCases.Infrastructure.AuditLogs;

public interface IAuditLogRepository : Repository
{
    void AddLog(AuditLog log);
}