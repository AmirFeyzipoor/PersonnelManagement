namespace PersonnelManagement.UseCases.Identities.Contracts.Dtos;

public class GetAllPersonnelDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreationDate { get; set; }
    public string? Email { get; set; }
}