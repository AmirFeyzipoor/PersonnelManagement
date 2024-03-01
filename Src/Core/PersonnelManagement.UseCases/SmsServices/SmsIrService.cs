using IPE.SmsIrClient;
using PersonnelManagement.UseCases.SmsServices.Configs;
using PersonnelManagement.UseCases.SmsServices.Contracts;

namespace PersonnelManagement.UseCases.SmsServices;

public class SmsIrService : ISmsService
{
    private readonly SmsSettings _smsSettings;

    public SmsIrService(SmsSettings smsSettings)
    {
        _smsSettings = smsSettings;
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

            throw new Exception(errorDescription);
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

            throw new Exception(errorDescription);
        }
    }
}