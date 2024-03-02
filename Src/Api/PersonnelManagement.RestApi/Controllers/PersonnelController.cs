using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonnelManagement.UseCases.Infrastructure.SortUtilities;
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

    public PersonnelController(
        IPersonnelService personnelService,
        UriSortParser sortParser,
        INotificationService notificationService)
    {
        _personnelService = personnelService;
        _sortParser = sortParser;
        _notificationService = notificationService;
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
        var user =  await _personnelService.RegisterUser(dto);

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
    public List<GetAllPersonnelDto> GetAll(
        [FromQuery] GetAllPersonnelFilterDto filter,
        [FromQuery] string? sort)
    {
        var sortExpression = !string.IsNullOrEmpty(sort) ?
            _sortParser.Parse<GetAllPersonnelDto>(sort) :
            null;
        
        return _personnelService.GetAll(filter, sortExpression);
    }
}