namespace PersonnelManagement.IntegrationTest.Infrastructure;

public class Scenario : Attribute
{
    public Scenario(string title)
    {
        Title = title;
    }

    public string Title { get; set; }
}