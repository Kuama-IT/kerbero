using System.Security.Claims;
using Kerbero.Identity.Library.Modules.Authentication.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
  public async Task<ActionResult> Login(LoginDto loginDto)
  {
    var authenticatedDto = await _authenticationService.Login(loginDto);
    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
      new ClaimsPrincipal(authenticatedDto.ClaimIdentity), authenticatedDto.Properties);

    return new OkResult();
  }

  [HttpPost("logout")]
  [AllowAnonymous]
  public async Task<ActionResult> Logout()
  {
    await _authenticationService.Logout();
    await HttpContext.SignOutAsync();

    return new OkResult();
  }
}