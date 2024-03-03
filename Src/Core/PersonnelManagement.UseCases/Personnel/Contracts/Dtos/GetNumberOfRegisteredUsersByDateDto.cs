namespace PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

public class GetNumberOfRegisteredUsersByDateDto
{
    public string Date { get; set; }
    public List<GetNumberOfRegisteredUsersByRegistrantDto> UsersCountByRegistrant { get; set; }
    public long Count { get; set; }
}