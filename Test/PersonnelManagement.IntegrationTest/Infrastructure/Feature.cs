namespace PersonnelManagement.IntegrationTest.Infrastructure;

public class Feature : Attribute
{
    public Feature(string title)
    {
        Title = title;
    }

    public string Title { get; set; }
    public string InOrderTo { get; set; }
    public string AsA { get; set; }
    public string IWantTo { get; set; }
}