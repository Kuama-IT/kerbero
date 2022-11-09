using System.Security.Claims;
using Kerbero.Identity.Modules.Authentication.Models;

namespace Kerbero.Identity.Modules.Authentication.Services;

public interface ITokenHelper
{
  public string GenerateAccessToken(IEnumerable<Claim> claims, string signKey, DateTime expires);
  RefreshTokenModel GenerateRefreshToken();
}