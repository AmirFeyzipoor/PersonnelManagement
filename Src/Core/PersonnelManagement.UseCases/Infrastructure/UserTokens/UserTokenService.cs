using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PersonnelManagement.UseCases.Infrastructure.UserTokens.Contrects;
using PersonnelManagement.UseCases.Infrastructure.UserTokens.Contrects.Exceptions;

namespace PersonnelManagement.UseCases.Infrastructure.UserTokens;

public class UserTokenService : IUserTokenService
{
    private readonly IHttpContextAccessor _accessor;

    public UserTokenService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string? UserId  => GetUserIdFromToken();
    
    private string? GetUserIdFromToken()
    {
        var userId = _accessor.HttpContext!.User.Claims
            .FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier)?.Value;

        StopIfUserIdIsNotValid(userId);

        return userId;
    }
    
    private static void StopIfUserIdIsNotValid(string? userId)
    {
        if (userId == null)
            throw new UserIdIsNotValidException();
    }
}