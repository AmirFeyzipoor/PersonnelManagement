namespace PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

public class GetNumberOfRegisteredUsersByRegistrantDto
{
    public string? RegisteredId { get; set; }
    public long Count { get; set; }
}