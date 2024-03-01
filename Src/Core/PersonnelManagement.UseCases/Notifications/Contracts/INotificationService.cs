using PersonnelManagement.UseCases.Infrastructure;

namespace PersonnelManagement.UseCases.Notifications.Contracts;

public interface INotificationService : Service
{
    Task BulkSendSms(string messageText, List<string> phoneNumbers);
    Task SendSms(string messageText, string phoneNumber);
    Task SendEmail(string toEmail,
        string subject,
        string message);
}