using System.Security.Claims;
using Kerbero.Identity.Common;
using Kerbero.Identity.Common.Exceptions;
using Kerbero.Identity.Modules.Roles.Services;
using Kerbero.Identity.Modules.Users.Entities;
using Kerbero.Identity.Modules.Users.Services;

namespace Kerbero.Identity.Modules.Authentication.Services;

public class AuthenticationHelper : IAuthenticationHelper
{
  private readonly ITokenHelper _tokenHelper;
  private readonly IUserManager _userManager;
  private readonly IRoleManager _roleManager;
  private readonly KerberoIdentityConfiguration _kerberoIdentityConfiguration;

  public AuthenticationHelper(
    ITokenHelper tokenHelper,
    IUserManager userManager,
    IRoleManager roleManager,
    KerberoIdentityConfiguration kerberoIdentityConfiguration
  )
  {
    _tokenHelper = tokenHelper;
    _userManager = userManager;
    _roleManager = roleManager;
    _kerberoIdentityConfiguration = kerberoIdentityConfiguration;
  }

  public async Task<string> GenerateAccessToken(User user)
  {
    // user id in claims
    var claims = new List<Claim>
    {
      new(ClaimTypes.Sid, user.Id.ToString()),
    };

    // user claims
    var userClaims = await _userManager.GetClaimsByUser(user);
    claims.AddRange(userClaims);

    var rolesNames = await _userManager.GetRolesNamesByUser(user);
    var roles = await _roleManager.GetByNames(rolesNames);

    // user roles claims
    foreach (var role in roles)
    {
      var roleClaims = await _roleManager.GetClaimsByRole(role);
      claims.AddRange(roleClaims);
    }

    var expireDate = DateTime.UtcNow.AddMinutes(_kerberoIdentityConfiguration.AccessTokenExpirationInMinutes);
    return _tokenHelper.GenerateAccessToken(claims, _kerberoIdentityConfiguration.AccessTokenSingKey, expireDate);
  }

  public async Task<string> GenerateAndSaveRefreshToken(User user)
  {
    var refreshTokenModel = _tokenHelper.GenerateRefreshToken();
    // save refresh token
    user.RefreshToken = refreshTokenModel.Token;
    user.RefreshTokenExpireDate = refreshTokenModel.ExpireDate;

    var result = await _userManager.Update(user);
    if (!result.Succeeded)
    {
      throw new InternalException();
    }

    return refreshTokenModel.Token;
  }
}