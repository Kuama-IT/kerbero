using Kerbero.Identity.Modules.Authentication.Services;
using Kerbero.Identity.Library.Common.Dtos;
using Kerbero.Identity.Library.Modules.Authentication.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

  [HttpPost("email")]
  [AllowAnonymous]
  public async Task<ActionResult<AuthenticatedDto>> LoginWithEmail(LoginEmailDto loginEmailDto)
  {
    return await _authenticationService.LoginWithEmail(loginEmailDto);
  }

  [HttpPost("refresh-token")]
  [AllowAnonymous]
  public async Task<ActionResult<AuthenticatedDto>> LoginWithRefreshToken(LoginRefreshTokenDto loginRefreshTokenDto)
  {
    return await _authenticationService.LoginWithRefreshToken(loginRefreshTokenDto);
  }
}