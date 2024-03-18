using System.Net;
using PersonnelManagement.UseCases.Infrastructure;

namespace PersonnelManagement.RestApi.Middelwares.CustomExceptionMiddleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private ILogger _logger;

    public ExceptionMiddleware(RequestDelegate next, 
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (BusinessException businessException)
        {
            await HandleExceptionAsync(httpContext, businessException);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
            _logger.LogError(ex.ToString());
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        
        var message = exception switch
        {
            BusinessException =>  exception.GetType().Name,
            _ => "Internal Server Error."
        };

        await context.Response.WriteAsync(new ExceptionDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = message
        }.ToString());
    }
}