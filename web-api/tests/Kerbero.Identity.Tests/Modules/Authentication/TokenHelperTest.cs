using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Kerbero.Identity.Common;
using Kerbero.Identity.Modules.Authentication.Services;
using Xunit;

namespace Kerbero.Identity.Tests.Modules.Authentication;

public class TokenHelperTest
{
  private readonly KerberoIdentityConfiguration _kerberoIdentityConfiguration = new();
  private readonly TokenHelper _tokenHelper;

  public TokenHelperTest()
  {
    _tokenHelper = new TokenHelper(_kerberoIdentityConfiguration);
  }

  [Fact]
  public void GenerateAccessToken_ValidInput_ReturnCorrectResult()
  {
    var tClaims = new List<Claim>
    {
      new("claimType1", "claimValue1"),
      new("claimType2", "claimValue2"),
    };

    var tSignKey = "test test test test test test test ";

    var tExpireDate = DateTime.UtcNow.AddDays(1);
    var token = _tokenHelper.GenerateAccessToken(tClaims, tSignKey, tExpireDate);

    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

    // should be in the 2 minute range from
    jwtSecurityToken.ValidTo.Should().BeCloseTo(tExpireDate, TimeSpan.FromMinutes(1));
    
    // should contain claims
    jwtSecurityToken.Claims.Single(e => e.Type == tClaims[0].Type).Value.Should().BeEquivalentTo(tClaims[0].Value);
    jwtSecurityToken.Claims.Single(e => e.Type == tClaims[1].Type).Value.Should().BeEquivalentTo(tClaims[1].Value);
  }
}