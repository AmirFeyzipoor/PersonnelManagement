namespace PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

public class GetUserCreationDateWithRegistrationIdDto
{
    public DateTime CreationDate { get; set; }
    public string? RegistrantId { get; set; }
}