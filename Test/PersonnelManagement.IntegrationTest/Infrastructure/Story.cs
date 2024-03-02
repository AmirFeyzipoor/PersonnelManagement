namespace PersonnelManagement.IntegrationTest.Infrastructure;

public class Story : Attribute
{
    public string Title { get; set; }
    public string InOrderTo { get; set; }
    public string AsA { get; set; }
    public string IWantTo { get; set; }
}