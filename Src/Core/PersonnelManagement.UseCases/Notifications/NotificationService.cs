using PersonnelManagement.UseCases.Notifications.EmailServices.Contracts;
using PersonnelManagement.UseCases.Notifications.SmsServices.Contracts;

namespace PersonnelManagement.UseCases.Notifications;

public class NotificationService : INotificationService
{
    private readonly ISmsService _smsService;
    private readonly IEmailService _emailService;

    public NotificationService(ISmsService smsService,
        IEmailService emailService)
    {
        _smsService = smsService;
        _emailService = emailService;
    }

    public async Task BulkSendSms(string messageText, List<string> phoneNumbers)
    {
        await _smsService.BulkSendSms(messageText, phoneNumbers);
    }
    
    public async Task SendSms(string messageText, string phoneNumber)
    {
        await _smsService.SendSms(messageText, phoneNumber);
    }

    public async Task SendEmail(string toEmail, string subject, string message)
    {
        await _emailService.SendEmailAsync(toEmail, subject, message);
    }
}