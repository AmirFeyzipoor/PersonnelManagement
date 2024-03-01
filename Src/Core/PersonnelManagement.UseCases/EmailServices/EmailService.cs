using System.Net;
using System.Net.Mail;
using PersonnelManagement.UseCases.EmailServices.Configs;
using PersonnelManagement.UseCases.EmailServices.Contracts;

namespace PersonnelManagement.UseCases.EmailServices;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    
    public EmailService(SmtpSettings smtpSettings)
    {
        _smtpSettings = smtpSettings;
    }
    
    public Task SendEmailAsync(
        string toEmail,
        string subject,
        string body)
    { 
        var message = GenerateMailMessage(toEmail,body,subject);
        var smtp = GenerateSmtp(); 
        
        smtp.Send(message); 
        
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