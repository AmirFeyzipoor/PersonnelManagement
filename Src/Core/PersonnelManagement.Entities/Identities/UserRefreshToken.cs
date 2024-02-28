namespace PersonnelManagement.Entities.Identities;

public class UserRefreshToken
{
    public string UserId { get; set; }
    public User User { get; set; }
    public string Token { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}