namespace PersonnelManagement.UseCases.EmailServices.Configs;

public class SmtpSettings
{
    public string MailAddress { get; set; }
    public string NetworkCredentialUserName { get; set; }
    public string Password { get; set; }
    public bool IsBodyHtml { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpHost { get; set; }
    public bool EnableSsl { get; set; }
    public bool UseDefaultCredentials { get; set; }
}