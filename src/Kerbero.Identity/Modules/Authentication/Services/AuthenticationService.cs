using Kerbero.Identity.Common.Exceptions;
using Kerbero.Identity.Modules.Users.Services;
using Kerbero.Identity.Library.Modules.Authentication.Dtos;

namespace Kerbero.Identity.Modules.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
  private readonly IUserManager _userManager;
  private readonly IAuthenticationManager _authenticationManager;
  private readonly IAuthenticationHelper _authenticationHelper;

  public AuthenticationService(
    IUserManager userManager,
    IAuthenticationManager authenticationManager,
    IAuthenticationHelper authenticationHelper
    )
  {
    _userManager = userManager;
    _authenticationManager = authenticationManager;
    _authenticationHelper = authenticationHelper;
  }

  public async Task<AuthenticatedDto> LoginWithEmail(LoginEmailDto loginEmailDto)
  {
    var user = await _userManager.FindByEmail(loginEmailDto.Email);
    if (user is null)
    {
      throw new UnauthorizedException();
    }

    var signInResult = await _authenticationManager.SignInWithPassword(user, loginEmailDto.Password);
    if (!signInResult.Succeeded)
    {
      throw new UnauthorizedException();
    }

    var accessToken = await _authenticationHelper.GenerateAccessToken(user);
    var refreshToken = await _authenticationHelper.GenerateAndSaveRefreshToken(user);

    return new AuthenticatedDto
    {
      AccessToken = accessToken,
      RefreshToken = refreshToken,
    };
  }

  public async Task<AuthenticatedDto> LoginWithRefreshToken(LoginRefreshTokenDto loginRefreshTokenDto)
  {
    var user = await _userManager.FindByRefreshToken(loginRefreshTokenDto.Token);
    if (user is null)
    {
      throw new UnauthorizedException();
    }

    // refresh token expired
    if (DateTime.UtcNow > user.RefreshTokenExpireDate)
    {
      throw new UnauthorizedException();
    }

    var accessToken = await _authenticationHelper.GenerateAccessToken(user);
    var refreshToken = await _authenticationHelper.GenerateAndSaveRefreshToken(user);

    return new AuthenticatedDto
    {
      AccessToken = accessToken,
      RefreshToken = refreshToken,
    };
  }
}