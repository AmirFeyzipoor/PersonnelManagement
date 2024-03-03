namespace PersonnelManagement.UseCases.Infrastructure.UserTokens.Contrects;

public interface IUserTokenService : Service
{
    string? UserId { get; }
}