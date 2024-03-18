using System.Text.Json;

namespace PersonnelManagement.RestApi.Middelwares.CustomExceptionMiddleware;

public class ExceptionDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}