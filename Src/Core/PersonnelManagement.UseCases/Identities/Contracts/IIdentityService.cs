using PersonnelManagement.UseCases.Identities.Contracts.Dtos;
using PersonnelManagement.UseCases.Infrastructure;

namespace PersonnelManagement.UseCases.Identities.Contracts;

public interface IIdentityService : Service
{
    Task<string> LoginUser(LoginUserDto dto);
    Task<string> RegisterUser(RegisterUserDto dto);
}