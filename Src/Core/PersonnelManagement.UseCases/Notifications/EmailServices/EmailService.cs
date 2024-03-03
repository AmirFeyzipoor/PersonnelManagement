using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using PersonnelManagement.UseCases.Notifications.EmailServices.Configs;
using PersonnelManagement.UseCases.Notifications.EmailServices.Contracts;

namespace PersonnelManagement.UseCases.Notifications.EmailServices;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger _logger;

    public EmailService(SmtpSettings smtpSettings,
        ILogger<EmailService> logger)
    {
        _smtpSettings = smtpSettings;
        _logger = logger;
    }

    public Task SendEmailAsync(
        string toEmail,
        string subject,
        string body)
    {
        try
        {
            var message = GenerateMailMessage(toEmail, body, subject);
            var smtp = GenerateSmtp();

            smtp.Send(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        return Task.CompletedTask;
    }

    private SmtpClient GenerateSmtp()
    {
        var smtp = new SmtpClient();
        smtp.Port = _smtpSettings.SmtpPort;
        smtp.Host = _smtpSettings.SmtpHost;
        smtp.EnableSsl = _smtpSettings.EnableSsl;
        smtp.UseDefaultCredentials = _smtpSettings.UseDefaultCredentials;
        smtp.Credentials = new NetworkCredential(
            _smtpSettings.NetworkCredentialUserName,
            _smtpSettings.Password);
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

        return smtp;
    }

    private MailMessage GenerateMailMessage(
        string toMailAddress,
        string body,
        string subject)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_smtpSettings.MailAddress);
        message.To.Add(new MailAddress(toMailAddress));
        message.Subject = subject;
        message.IsBodyHtml = _smtpSettings.IsBodyHtml;
        message.Body = body;
        return message;
    }
}