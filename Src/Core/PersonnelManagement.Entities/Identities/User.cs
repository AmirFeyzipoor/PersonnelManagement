using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PersonnelManagement.Entities.Identities;

public class User : IdentityUser<string>
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreationDate { get; set; }
    public string? Email { get; set; }
    
    [Timestamp]
    public string? ConcurrencyStamp { get; set; }
}