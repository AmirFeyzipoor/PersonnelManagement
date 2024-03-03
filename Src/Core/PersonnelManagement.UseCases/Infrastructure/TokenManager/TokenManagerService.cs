using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PersonnelManagement.UseCases.Infrastructure.TokenManager.Contracts;
using PersonnelManagement.UseCases.Personnel.Contracts.Configs;

namespace PersonnelManagement.UseCases.Infrastructure.TokenManager;

public class TokenManagerService : ITokenManagerService
{
    private readonly JwtBearerTokenSettings _jwtBearerTokenSettings;

    public TokenManagerService(JwtBearerTokenSettings jwtBearerTokenSettings)
    {
        _jwtBearerTokenSettings = jwtBearerTokenSettings;
    }

    public string GenerateToken(string userId, List<string> userRoles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtBearerTokenSettings.SecretKey);

        var tokenClaims = new ClaimsIdentity();
        tokenClaims.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));

        WriteUserRolesToTokenClaims(ref tokenClaims, userRoles);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = tokenClaims,

            Expires = DateTime.UtcNow.AddSeconds(_jwtBearerTokenSettings.ExpiryTimeInSeconds),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Audience = _jwtBearerTokenSettings.Audience,
            Issuer = _jwtBearerTokenSettings.Issuer
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    private static void WriteUserRolesToTokenClaims(
        ref ClaimsIdentity tokenClaims,
        IEnumerable<string> userRoles)
    {
        foreach (var role in userRoles)
        {
            tokenClaims.AddClaim(new Claim(ClaimTypes.Role, role));
        }
    }
}