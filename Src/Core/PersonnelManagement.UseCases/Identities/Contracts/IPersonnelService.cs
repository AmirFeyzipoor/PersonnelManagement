using PersonnelManagement.UseCases.Identities.Contracts.Dtos;
using PersonnelManagement.UseCases.Infrastructure;
using PersonnelManagement.UseCases.Infrastructure.SortUtilities;

namespace PersonnelManagement.UseCases.Identities.Contracts;

public interface IPersonnelService : Service
{
    Task<string> LoginUser(LoginUserDto dto);
    Task<string> RegisterUser(RegisterPersonnelDto dto);
    List<GetAllPersonnelDto> GetAll(
        GetAllPersonnelFilterDto filter, 
        ISort<GetAllPersonnelDto>? sort);
}