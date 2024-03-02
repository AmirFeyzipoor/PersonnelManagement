using IPE.SmsIrClient;
using Microsoft.Extensions.Logging;
using PersonnelManagement.UseCases.Notifications.SmsServices.Configs;
using PersonnelManagement.UseCases.Notifications.SmsServices.Contracts;

namespace PersonnelManagement.UseCases.Notifications.SmsServices;

public class SmsIrService : ISmsService
{
    private readonly SmsSettings _smsSettings;
    private readonly ILogger _logger;

    public SmsIrService(SmsSettings smsSettings,
        ILogger<SmsIrService> logger)
    {
        _smsSettings = smsSettings;
        _logger = logger;
    }

    public async Task BulkSendSms(string messageText, List<string> phoneNumbers)
    {
        try
        {
            var smsIr = new SmsIr(_smsSettings.ApiKey);
            
            await smsIr.BulkSendAsync(
                long.Parse(_smsSettings.LineNumber),
                messageText,
                phoneNumbers.ToArray());
        }
        catch (Exception ex)
        {
            var errorName = ex.GetType().Name;
            var errorNameDescription = errorName switch
            {
                "UnauthorizedException" => "The provided token is not valid or access is denied.",
                "LogicalException" => "Please check and correct the request parameters.",
                "TooManyRequestException" => "The request count has exceeded the allowed limit.",
                "UnexpectedException" or "InvalidOperationException" => "An unexpected error occurred on the remote server.",
                _ => "Unable to send the request due to an unspecified error.",
            };

            var errorDescription = "There is a problem with the request." +
                $"\n - Error: {errorName} - {errorNameDescription} - {ex.Message}";

            _logger.LogError(errorDescription);
        }
    }

    public async Task SendSms(string messageText, string phoneNumber)
    {
        try
        {
            var smsIr = new SmsIr(_smsSettings.ApiKey);
            
            await smsIr.BulkSendAsync(
                long.Parse(_smsSettings.LineNumber),
                messageText, 
                new[] {phoneNumber});
        }
        catch (Exception ex)
        {
            var errorName = ex.GetType().Name;
            var errorNameDescription = errorName switch
            {
                "UnauthorizedException" => "The provided token is not valid or access is denied.",
                "LogicalException" => "Please check and correct the request parameters.",
                "TooManyRequestException" => "The request count has exceeded the allowed limit.",
                "UnexpectedException" or "InvalidOperationException" => "An unexpected error occurred on the remote server.",
                _ => "Unable to send the request due to an unspecified error.",
            };

            var errorDescription = "There is a problem with the request." +
                                   $"\n - Error: {errorName} - {errorNameDescription} - {ex.Message}";

            _logger.LogError(errorDescription);
        }
    }
}