using Kerbero.Identity.Library.Modules.Authentication.Dtos;
using Kerbero.Identity.Library.Modules.Users.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthenticationService = Kerbero.Identity.Modules.Authentication.Services.IAuthenticationService;

namespace Kerbero.Identity.Modules.Authentication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
  private readonly IAuthenticationService _authenticationService;

  public AuthenticationController(IAuthenticationService authenticationService)
  {
    _authenticationService = authenticationService;
  }

  [HttpPost("login")]
  [AllowAnonymous]
  public async Task<ActionResult<UserReadDto>> Login(LoginDto loginDto)
  {
    return await _authenticationService.Login(loginDto);
  }

  [HttpPost("logout")]
  [AllowAnonymous]
  public async Task<ActionResult> Logout()
  {
    await _authenticationService.Logout();
    await HttpContext.SignOutAsync();

    return Ok();
  }
}
