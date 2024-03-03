using Dapper;
using PersonnelManagement.UseCases.Personnel.Contracts;
using PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

namespace PersonnelManagement.Infrastructure.Data.Identities;

public class DapperPersonnelRepository : IPersonnelRepository
{
    private readonly DapperDataContext _context;

    public DapperPersonnelRepository(DapperDataContext context)
    {
        _context = context; 
    }

    public async Task<List<GetUserCreationDateWithRegistrationIdDto>> GetAllUserCreationDateWithRegistrantId()
    {
        const string query =
            @"SELECT [u].[CreationDate], [u].[RegistrantId]
                FROM [Users] AS [u]";

        using var connection = _context.CreateConnection();
        
        return (List<GetUserCreationDateWithRegistrationIdDto>)await connection
            .QueryAsync<GetUserCreationDateWithRegistrationIdDto>(query);
    }
}