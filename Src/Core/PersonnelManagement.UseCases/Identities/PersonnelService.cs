using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PersonnelManagement.Entities.Identities;
using PersonnelManagement.UseCases.Identities.Contracts;
using PersonnelManagement.UseCases.Identities.Contracts.Configs;
using PersonnelManagement.UseCases.Identities.Contracts.Dtos;
using PersonnelManagement.UseCases.Identities.Contracts.Exceptions;
using PersonnelManagement.UseCases.Infrastructure.SortUtilities;

namespace PersonnelManagement.UseCases.Identities;

public class PersonnelService : IPersonnelService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtBearerTokenSettings _jwtBearerTokenSettings;

    public PersonnelService(UserManager<User> userManager,
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

        var userRoles = await _userManager.GetRolesAsync(user!);

        return GenerateToken(user!.Id, userRoles);
    }

    public async Task<string> RegisterUser(RegisterPersonnelDto dto)
    {
        StopIfDuplicatedPhoneNumber(dto.PhoneNumber);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name,
            UserName = dto.Name + Guid.NewGuid(),
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            CreationDate = DateTime.Now.ToUniversalTime()
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        StopIfCreateUserFailed(result);

        return user.Id;
    }

    public List<GetAllPersonnelDto> GetAll(
        GetAllPersonnelFilterDto filter,
        ISort<GetAllPersonnelDto>? sort)
    {
        var personnel = _userManager.Users
            .Select(_ => new GetAllPersonnelDto
            {
                Name = _.Name,
                LastName = _.LastName,
                PhoneNumber = _.PhoneNumber,
                Email = _.Email,
                CreationDate = _.CreationDate
            });

        personnel = DoFilterOnPersonnel(filter, personnel);
        
        if (sort != null)
            personnel = personnel.Sort(sort);

        return personnel.ToList();
    }

    private static IQueryable<GetAllPersonnelDto> DoFilterOnPersonnel(
        GetAllPersonnelFilterDto filter,
        IQueryable<GetAllPersonnelDto> personnel)
    {
        if (filter?.Name != null)
        {
            personnel = personnel.Where(
                _ => _.Name == filter.Name);
        }
        
        if (filter?.LastName != null)
        {
            personnel = personnel.Where(
                _ => _.LastName == filter.LastName);
        }

        return personnel;
    }

    private void StopIfDuplicatedPhoneNumber(string phoneNumber)
    {
        var isDuplicatedPhoneNumber = _userManager.Users
            .Any(_ => _.PhoneNumber == phoneNumber);
        if (isDuplicatedPhoneNumber)
            throw new DuplicatedPhoneNumberException();
    }

    private static void StopIfCreateUserFailed(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new FailedCreateUserException();
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

    private string GenerateToken(string userId, IList<string> userRoles)
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