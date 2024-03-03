namespace PersonnelManagement.UseCases.Infrastructure.TokenManager.Contracts;

public interface ITokenManagerService : Service
{
    string GenerateToken(string userId, List<string> userRoles);
}