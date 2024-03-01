using PersonnelManagement.UseCases.Infrastructure;

namespace PersonnelManagement.UseCases.SmsServices.Contracts;

public interface ISmsService : Service
{
    Task BulkSendSms(string messageText, List<string> phoneNumbers);
    Task SendSms(string messageText, string phoneNumber);
}