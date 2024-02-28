namespace PersonnelManagement.UseCases.Configurations;

public class SeedDataConfigs
{
    public List<string> ApplicationRoles { get; set; }
    public List<AdminConfig> Admins { get; set; }
}

public class AdminConfig
{
    public string AdminName { get; set; }
    public string AdminLastName { get; set; }
    public string AdminPhoneNumber { get; set; }
    public string? AdminEmail { get; set; }
    public string AdminPassword { get; set; }
}   