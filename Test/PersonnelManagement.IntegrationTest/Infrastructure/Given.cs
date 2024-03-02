namespace PersonnelManagement.IntegrationTest.Infrastructure;

public class Given : Attribute
{
    public Given(string description)
    {
        Description = description;
    }

    public string Description { get; set; }
}