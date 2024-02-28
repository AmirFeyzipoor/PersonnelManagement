using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonnelManagement.UseCases.Identities.Contracts;
using PersonnelManagement.UseCases.Identities.Contracts.Dtos;

namespace PersonnelManagement.RestApi.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentitiesController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public IdentitiesController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        return Ok(new
        {
            AccessToken = await _identityService.LoginUser(dto)
        });
    }
    
    [HttpPost("register-personnel")]
    [Authorize(Roles = "Admin")]
    public async Task<string> RegisterUser(RegisterUserDto dto)
    {
        return await _identityService.RegisterUser(dto);
    }
}