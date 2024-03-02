namespace PersonnelManagement.IntegrationTest.Infrastructure;

public class When : Attribute
{
    public When(string description)
    {
        Description = description;
    }

    public string Description { get; set; }
}