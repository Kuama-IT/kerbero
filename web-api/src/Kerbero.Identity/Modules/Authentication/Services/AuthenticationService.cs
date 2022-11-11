using System.Security.Claims;
using Kerbero.Identity.Common.Exceptions;
using Kerbero.Identity.Modules.Users.Services;
using Kerbero.Identity.Library.Modules.Authentication.Dtos;
using Kerbero.Identity.Modules.Authentication.Models;
using Kerbero.Identity.Modules.Users.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Kerbero.Identity.Modules.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
  private readonly IUserManager _userManager;
  private readonly IAuthenticationManager _authenticationManager;

  public AuthenticationService(
    IUserManager userManager,
    IAuthenticationManager authenticationManager
    )
  {
    _userManager = userManager;
    _authenticationManager = authenticationManager;
  }

  public async Task<AuthenticatedModel> Login(LoginDto loginDto)
  {
    var user = await _userManager.FindByEmail(loginDto.Email);
    if (user is null)
    {
      throw new UnauthorizedException();
    }

    var signInResult = await _authenticationManager.SignInWithPassword(user, loginDto.Password);
    if (!signInResult.Succeeded)
    {
      throw new UnauthorizedException();
    }

    var claimIdentity = new ClaimsIdentity(
      new List<Claim>()
      {
        new ( ClaimTypes.Sid, user.Id.ToString() )
      }, CookieAuthenticationDefaults.AuthenticationScheme);

    var authProperties = new AuthenticationProperties
    {
      //AllowRefresh = <bool>,
      // Refreshing the authentication session should be allowed.

      //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
      // The time at which the authentication ticket expires. A 
      // value set here overrides the ExpireTimeSpan option of 
      // CookieAuthenticationOptions set with AddCookie.

      IsPersistent = true,
      // Whether the authentication session is persisted across 
      // multiple requests. When used with cookies, controls
      // whether the cookie's lifetime is absolute (matching the
      // lifetime of the authentication ticket) or session-based.

      //IssuedUtc = <DateTimeOffset>,
      // The time at which the authentication ticket was issued.

      //RedirectUri = <string>
      // The full path or absolute URI to be used as an http 
      // redirect response value.
    }; // keep as reference

    return new AuthenticatedModel()
    {
      Properties = authProperties,
      ClaimIdentity = claimIdentity
    };

  }
  
  public async Task Logout()
  {
    await _authenticationManager.SignOut();
  }
  
}