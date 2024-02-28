using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PersonnelManagement.Entities.Identities;
using PersonnelManagement.UseCases.Identities.Contracts;
using PersonnelManagement.UseCases.Identities.Contracts.Dtos;
using PersonnelManagement.UseCases.Identities.Contracts.Exceptions;
using PersonnelManagement.UseCases.Identities.Contracts.TokenConfigs;

namespace PersonnelManagement.UseCases.Identities;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtBearerTokenSettings _jwtBearerTokenSettings;

    public IdentityService(UserManager<User> userManager,
        JwtBearerTokenSettings jwtBearerTokenSettings)
    {
        _userManager = userManager;
        _jwtBearerTokenSettings = jwtBearerTokenSettings;
    }

    public async Task<string> LoginUser(LoginUserDto dto)
    {
        var user = _userManager.Users.FirstOrDefault(_ => _.PhoneNumber == dto.PhoneNumber);
        
        StopIfUserNotFound(user);

        await StopIfWrongUserNameOrPassword(dto.Password, user!);
        
        return GenerateToken(user!.Id);
    }

    private static void StopIfUserNotFound(User? user)
    {
        if (user == null)
            throw new UserNotFoundException();
    }

    private async Task StopIfWrongUserNameOrPassword(string password, User user)
    {
        var isCorrectPassword = await _userManager.CheckPasswordAsync(user, password);
        if (isCorrectPassword == false)
            throw new WrongUserNameOrPasswordException();
    }
    
    private string GenerateToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtBearerTokenSettings.SecretKey);

        var tokenClaims = new ClaimsIdentity();
        tokenClaims.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));

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
}