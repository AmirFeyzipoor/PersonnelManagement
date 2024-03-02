namespace PersonnelManagement.IntegrationTest.Infrastructure;

public class Then : Attribute
{
    public Then(string description)
    {
        Description = description;
    }

    public string Description { get; set; }
}