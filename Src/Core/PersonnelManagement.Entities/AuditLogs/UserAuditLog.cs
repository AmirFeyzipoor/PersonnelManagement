using PersonnelManagement.Entities.Identities;

namespace PersonnelManagement.Entities.AuditLogs;

public class UserAuditLog
{
    public int Id { get; set; }
    public required string RegistrantId { get; set; }
    public DateTime Timestamp { get; set; }
    public required string Action { get; set; }
    public required string Changes { get; set; }
    public User User { get; set; }
    public string UserId { get; set; }
}