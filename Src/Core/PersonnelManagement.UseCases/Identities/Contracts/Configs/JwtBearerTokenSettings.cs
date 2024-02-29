namespace PersonnelManagement.UseCases.Identities.Contracts.Configs;

public class JwtBearerTokenSettings
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string SecretKey { get; set; }
    public double ExpiryTimeInSeconds { get; set; }
}