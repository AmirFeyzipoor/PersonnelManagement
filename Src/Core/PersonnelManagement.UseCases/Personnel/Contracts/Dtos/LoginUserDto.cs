namespace PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

public class LoginUserDto
{
    public LoginUserDto(string phoneNumber, string password)
    {
        PhoneNumber = phoneNumber;
        Password = password;
    }

    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}