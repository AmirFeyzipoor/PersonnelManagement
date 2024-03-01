using PersonnelManagement.UseCases.Infrastructure;

namespace PersonnelManagement.UseCases.EmailServices.Contracts;

public interface IEmailService : Service
{
    Task SendEmailAsync(
        string toEmail,
        string subject,
        string message);
}