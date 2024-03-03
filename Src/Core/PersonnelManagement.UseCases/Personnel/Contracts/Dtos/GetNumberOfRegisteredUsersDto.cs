namespace PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

public class GetNumberOfRegisteredUsersDto
{
    public List<GetNumberOfRegisteredUsersByDateDto> UsersCountByDate { get; set; }
    public long TotalCount { get; set; }
}