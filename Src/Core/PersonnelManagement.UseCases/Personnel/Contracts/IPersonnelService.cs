using PersonnelManagement.Entities.Identities;
using PersonnelManagement.UseCases.Infrastructure;
using PersonnelManagement.UseCases.Infrastructure.SortUtilities;
using PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

namespace PersonnelManagement.UseCases.Personnel.Contracts;

public interface IPersonnelService : Service
{
    Task<string> LoginUser(LoginUserDto dto);
    Task<User> RegisterUser(RegisterPersonnelDto dto);
    List<GetAllPersonnelDto> GetAll(
        GetAllPersonnelFilterDto filter, 
        ISort<GetAllPersonnelDto>? sort);
}