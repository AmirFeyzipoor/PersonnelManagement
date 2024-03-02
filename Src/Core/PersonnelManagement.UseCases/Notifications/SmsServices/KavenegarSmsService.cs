using Kavenegar.Models.Enums;
using Microsoft.Extensions.Logging;
using PersonnelManagement.UseCases.Notifications.SmsServices.Configs;
using PersonnelManagement.UseCases.Notifications.SmsServices.Contracts;

namespace PersonnelManagement.UseCases.Notifications.SmsServices;

public class KavenegarSmsService : ISmsService
{
    private readonly SmsSettings _smsSettings;
    private readonly ILogger _logger;

    public KavenegarSmsService(
        SmsSettings smsSettings,
        ILogger<KavenegarSmsService> logger)
    {
        _smsSettings = smsSettings;
        _logger = logger;
    }

    public Task BulkSendSms(string messageText, List<string> phoneNumbers)
    {
        try
        {
            // Due to the use of Kaveh Negar test panel, only messages sent to the number 09389066817 can be done.
            Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi(_smsSettings.ApiKey);
            api.Send(
                _smsSettings.LineNumber,
                phoneNumbers,
                messageText,
                MessageType.MobileMemory,
                DateTime.MinValue);
        }
        catch (Kavenegar.Exceptions.ApiException ex)
        {
            _logger.LogError(ex.Message);
        }
        catch (Kavenegar.Exceptions.HttpException ex)
        {
            _logger.LogError(ex.Message);
        }

        return Task.CompletedTask;
    }

    public Task SendSms(string messageText, string phoneNumber)
    {
        try
        {
            Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi(_smsSettings.ApiKey);
            api.Send(
                _smsSettings.LineNumber,
                phoneNumber,
                messageText);
        }
        catch (Kavenegar.Exceptions.ApiException ex)
        {
            _logger.LogError(ex.Message);
        }
        catch (Kavenegar.Exceptions.HttpException ex)
        {
            _logger.LogError(ex.Message);
        }

        return Task.CompletedTask;
    }
}