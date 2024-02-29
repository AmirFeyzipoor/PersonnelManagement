using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonnelManagement.UseCases.Identities.Contracts;
using PersonnelManagement.UseCases.Identities.Contracts.Dtos;
using PersonnelManagement.UseCases.Infrastructure.SortUtilities;

namespace PersonnelManagement.RestApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonnelController : ControllerBase
{
    private readonly IPersonnelService _personnelService;
    private readonly UriSortParser _sortParser;

    public PersonnelController(IPersonnelService personnelService,
        UriSortParser sortParser)
    {
        _personnelService = personnelService;
        _sortParser = sortParser;
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
        return await _personnelService.RegisterUser(dto);
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