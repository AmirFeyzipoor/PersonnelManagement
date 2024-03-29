using System.ComponentModel.DataAnnotations;

namespace PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

public class RegisterPersonnelDto
{
    public RegisterPersonnelDto(string name, string lastName, 
        string phoneNumber, string password)
    {
        Name = name;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Password = password;
    }

    [Required]
    public string Name { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Password { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
}