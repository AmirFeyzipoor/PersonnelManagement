using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonnelManagement.UseCases.Infrastructure.PaginationUtilities;
using PersonnelManagement.UseCases.Infrastructure.SortUtilities;
using PersonnelManagement.UseCases.Infrastructure.UserTokens.Contrects;
using PersonnelManagement.UseCases.Notifications;
using PersonnelManagement.UseCases.Personnel.Contracts;
using PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

namespace PersonnelManagement.RestApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonnelController : ControllerBase
{
    private readonly IPersonnelService _personnelService;
    private readonly UriSortParser _sortParser;
    private readonly INotificationService _notificationService;
    private readonly IUserTokenService _userTokenService;

    public PersonnelController(
        IPersonnelService personnelService,
        UriSortParser sortParser,
        INotificationService notificationService, 
        IUserTokenService userTokenService)
    {
        _personnelService = personnelService;
        _sortParser = sortParser;
        _notificationService = notificationService;
        _userTokenService = userTokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        return Ok(new
        {
            AccessToken = await _personnelService.LoginUser(dto)
        });
    }
    
    [HttpPost("register-personnel")]
    [Authorize(Roles = "Admin")]
    public async Task<string> RegisterPersonnel(RegisterPersonnelDto dto)
    {
        var userId = _userTokenService.UserId;
        
        var user =  await _personnelService.RegisterUser(userId!, dto);

        await _notificationService.SendSms(
            messageText: "با عرض خوش امدگویی ، ثبت نام شما" +
                         "در سامانه ی مدیریت پرسنل با موفقیت انجام شد",
            phoneNumber: user.PhoneNumber);

        if (user.Email != null)
        {
            await _notificationService.SendEmail(
                toEmail: user.Email,
                subject: $"Dear {user.Name} , welcome to the personnel Management system",
                message: "Welcome, your registration in the personnel management system" +
                         " has been done successfully");
        }
        
        return user.Id;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IPageResult<GetAllPersonnelDto>> GetAll(
        [FromQuery] GetAllPersonnelFilterDto filter,
        [FromQuery] string? sort,
        [FromQuery] int? limit,
        [FromQuery] int? offset)
    {
        var sortExpression = !string.IsNullOrEmpty(sort) ?
            _sortParser.Parse<GetAllPersonnelDto>(sort) :
            null;
        
        var pagination = limit.HasValue && offset.HasValue ?
            Pagination.Of(offset.Value + 1, limit.Value) :
            null;
        
        return await _personnelService.GetAll(filter, sortExpression, pagination);
    }
    
    [HttpGet("get_number_of_registered_users")]
    [Authorize(Roles = "Admin")]
    public async Task<GetNumberOfRegisteredUsersDto> GetNumberOfRegisteredUsers()
    {
        return await _personnelService.GetNumberOfRegisteredUsers();
    }
}