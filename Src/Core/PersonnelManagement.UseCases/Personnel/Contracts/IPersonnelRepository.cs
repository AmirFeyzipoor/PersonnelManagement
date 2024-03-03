using PersonnelManagement.UseCases.Infrastructure;
using PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

namespace PersonnelManagement.UseCases.Personnel.Contracts;

public interface IPersonnelRepository : Repository
{
    Task<List<GetUserCreationDateWithRegistrationIdDto>> GetAllUserCreationDateWithRegistrantId();
}