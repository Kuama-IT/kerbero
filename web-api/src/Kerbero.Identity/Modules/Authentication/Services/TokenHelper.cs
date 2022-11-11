using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kerbero.Identity.Common;
using Kerbero.Identity.Modules.Authentication.Models;
using Microsoft.IdentityModel.Tokens;

namespace Kerbero.Identity.Modules.Authentication.Services;

public class TokenHelper : ITokenHelper
{
  private readonly KerberoIdentityConfiguration _kerberoIdentityConfiguration;

  public TokenHelper(KerberoIdentityConfiguration kerberoIdentityConfiguration)
  {
    _kerberoIdentityConfiguration = kerberoIdentityConfiguration;
  }

  public string GenerateAccessToken(IEnumerable<Claim> claims, string signKey, DateTime expires)
  {
    // throws if expire date is less than "now"
    
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = expires,
      SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signKey)), SecurityAlgorithms.HmacSha256Signature
      )
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
    var token = tokenHandler.WriteToken(securityToken);

    return token;
  }

  public RefreshTokenModel GenerateRefreshToken()
  {
    var token = "" + Guid.NewGuid() + Guid.NewGuid();
    var expireDate = DateTime.UtcNow.AddMinutes(_kerberoIdentityConfiguration.RefreshTokenExpirationInMinutes);
    return new RefreshTokenModel(token, expireDate);
  }
}