using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using PersonnelManagement.Entities.AuditLogs;
using PersonnelManagement.Entities.Identities;
using PersonnelManagement.UseCases.Infrastructure;
using PersonnelManagement.UseCases.Infrastructure.AuditLogs;
using PersonnelManagement.UseCases.Infrastructure.SortUtilities;
using PersonnelManagement.UseCases.Infrastructure.TokenManager.Contracts;
using PersonnelManagement.UseCases.Personnel.Contracts;
using PersonnelManagement.UseCases.Personnel.Contracts.Configs;
using PersonnelManagement.UseCases.Personnel.Contracts.Dtos;
using PersonnelManagement.UseCases.Personnel.Contracts.Exceptions;
using PersonnelManagement.UseCases.Personnel.Exceptions;

namespace PersonnelManagement.UseCases.Personnel;

public class PersonnelService : IPersonnelService
{
    private readonly UserManager<User> _userManager;
    private readonly IPersonnelRepository _personnelRepository;
    private readonly ITokenManagerService _tokenManagerService;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    private static readonly object _lock = new();

    public PersonnelService(UserManager<User> userManager,
        IPersonnelRepository personnelRepository, 
        ITokenManagerService tokenManagerService, 
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _personnelRepository = personnelRepository;
        _tokenManagerService = tokenManagerService;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> LoginUser(LoginUserDto dto)
    {
        var user = _userManager.Users.FirstOrDefault(_ => _.PhoneNumber == dto.PhoneNumber);

        StopIfUserNotFound(user);

        await StopIfWrongUserNameOrPassword(dto.Password, user!);

        var userRoles = await _userManager.GetRolesAsync(user!);

        return _tokenManagerService.GenerateToken(user!.Id, userRoles.ToList());
    }

    public Task<User> RegisterUser(string userId, RegisterPersonnelDto dto)
    {
        StopIfInvalidPhoneNumber(dto.PhoneNumber);

        var user = GenerateUser(userId, dto);
        
        lock (_lock)
        {
            StopIfDuplicatedPhoneNumber(dto.PhoneNumber);
            
            var result = _userManager.CreateAsync(user, dto.Password);
            StopIfCreateUserFailed(result.Result);
        }

        GenerateLog(userId, user);

        _unitOfWork.Complete();

        return Task.FromResult(user);
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

    public async Task<GetNumberOfRegisteredUsersDto> GetNumberOfRegisteredUsers()
    {
        var users = await 
            _personnelRepository.GetAllUserCreationDateWithRegistrantId();
        return new GetNumberOfRegisteredUsersDto()
        {
            TotalCount = users.Count(),
            UsersCountByDate = users.Select(_ => new
                {
                    Day = _.CreationDate.ToString("dd/MM/yyyy"), _.RegistrantId
                })
                .GroupBy(_ => _.Day)
                .Select(_ => new GetNumberOfRegisteredUsersByDateDto()
                {
                    Date = _.Key,
                    Count = _.Count(),
                    UsersCountByRegistrant = _.GroupBy(_ => _.RegistrantId)
                        .Select(_ => new GetNumberOfRegisteredUsersByRegistrantDto
                        {
                            RegisteredId = _.Key,
                            Count = _.Count(),
                        }).ToList()
                }).ToList()
        };
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

    private static void StopIfInvalidPhoneNumber(string phoneNumber)
    {
        var mobileReg = @"^(0|0098|\+98)9(0[1-5]|[1 3]\d|2[0-2]|98)\d{7}$";
        var isValidMobileNumber = Regex.Match(phoneNumber, mobileReg);
        if (!isValidMobileNumber.Success)
        {
            throw new InvalidPhoneNumberException();
        }
    }

    public static string RemoveWhitespace(string input)
    {
        return new string(input.ToCharArray()
            .Where(c => !Char.IsWhiteSpace(c))
            .ToArray());
    }
    
    private void GenerateLog(string userId, User user)
    {
        var log = new AuditLog
        {
            UserId = userId,
            EntityName = user.GetType().Name,
            EntityPrimaryKey = user.Id,
            Action = EntityState.Added.ToString(),
            Timestamp = DateTime.UtcNow,
            Changes = new StringBuilder().ToString()
        };
        _auditLogRepository.AddLog(log);
    }
    
    private static User GenerateUser(string userId, RegisterPersonnelDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name,
            UserName = RemoveWhitespace(dto.Name + Guid.NewGuid()),
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            CreationDate = DateTime.Now.ToUniversalTime(),
            RegistrantId = userId
        };
        return user;
    }
}