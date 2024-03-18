using PersonnelManagement.Entities.Identities;
using PersonnelManagement.UseCases.Infrastructure;
using PersonnelManagement.UseCases.Infrastructure.PaginationUtilities;
using PersonnelManagement.UseCases.Infrastructure.SortUtilities;
using PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

namespace PersonnelManagement.UseCases.Personnel.Contracts;

public interface IPersonnelService : Service
{
    Task<string> LoginUser(LoginUserDto dto);
    Task<User> RegisterUser(string registrantId, RegisterPersonnelDto dto);
    Task<IPageResult<GetAllPersonnelDto>> GetAll(GetAllPersonnelFilterDto filter,
        ISort<GetAllPersonnelDto>? sort,
        Pagination? pagination);

    Task<GetNumberOfRegisteredUsersDto> GetNumberOfRegisteredUsers();
}