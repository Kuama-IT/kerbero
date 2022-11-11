using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Kerbero.Identity.Common;
using Kerbero.Identity.Common.Exceptions;
using Kerbero.Identity.Modules.Authentication.Models;
using Kerbero.Identity.Modules.Authentication.Services;
using Kerbero.Identity.Modules.Roles.Entities;
using Kerbero.Identity.Modules.Roles.Services;
using Kerbero.Identity.Modules.Users.Entities;
using Kerbero.Identity.Modules.Users.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using Range = Moq.Range;

namespace Kerbero.Identity.Tests.Modules.Authentication;

public class AuthenticationHelperTest
{
  private readonly Mock<IUserManager> _userManagerMock = new();
  private readonly Mock<IRoleManager> _roleManagerMock = new();
  private readonly Mock<ITokenHelper> _tokenHelperMock = new();
  private readonly KerberoIdentityConfiguration _kerberoIdentityConfiguration = new();

  private readonly AuthenticationHelper _authenticationHelper;

  public AuthenticationHelperTest()
  {
    _authenticationHelper = new AuthenticationHelper(
      _tokenHelperMock.Object,
      _userManagerMock.Object,
      _roleManagerMock.Object,
      _kerberoIdentityConfiguration
    );
  }

  [Fact]
  public async Task GenerateAccessToken_ValidInput_ReturnCorrectResult()
  {
    var tUser = new User { Id = Guid.NewGuid() };

    var tUserClaims = new List<Claim>
    {
      new("adpersonam", "can"),
    };

    var tUserRolesNames = new List<string> { "pubblisher" };
    var tUserRoles = new List<Role> { new Role { Name = "pubblisher" } };
    var tRoleClaims = new List<Claim>
    {
      new("role-permission", "specific"),
    };

    IEnumerable<Claim> tAllClaims = null!;

    var tToken = "mocked token";

    _userManagerMock
      .Setup(e => e.GetClaimsByUser(It.IsAny<User>()))
      .ReturnsAsync(tUserClaims);

    _userManagerMock
      .Setup(e => e.GetRolesNamesByUser(It.IsAny<User>()))
      .ReturnsAsync(tUserRolesNames);

    _roleManagerMock
      .Setup(e => e.GetByNames(It.IsAny<IEnumerable<string>>()))
      .ReturnsAsync(tUserRoles);

    // one role
    _roleManagerMock
      .Setup(e => e.GetClaimsByRole(It.IsAny<Role>()))
      .ReturnsAsync(tRoleClaims);

    _tokenHelperMock
      .Setup(e => e.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
      .Callback<IEnumerable<Claim>, string, DateTime>((claims, _, _) => { tAllClaims = claims; })
      .Returns(tToken);

    var actual = await _authenticationHelper.GenerateAccessToken(tUser);

    actual.Should().BeEquivalentTo(tToken);

    _userManagerMock.Verify(e => e.GetClaimsByUser(tUser));
    _userManagerMock.Verify(e => e.GetRolesNamesByUser(tUser));
    _roleManagerMock.Verify(e => e.GetByNames(tUserRolesNames));
    _roleManagerMock.Verify(e => e.GetClaimsByRole(tUserRoles.First()));


    var expireDate = DateTime.UtcNow.AddMinutes(_kerberoIdentityConfiguration.AccessTokenExpirationInMinutes);
    _tokenHelperMock.Verify(e =>
      e.GenerateAccessToken(tAllClaims, _kerberoIdentityConfiguration.AccessTokenSingKey,
        It.IsInRange(expireDate.AddMinutes(-1), expireDate.AddMinutes(1), Range.Inclusive)));

    _userManagerMock.VerifyNoOtherCalls();
    _roleManagerMock.VerifyNoOtherCalls();
    _tokenHelperMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task GenerateRefreshToken_ValidInput_ReturnCorrectResult()
  {
    var tUser = new User { Id = Guid.NewGuid() };

    var tRefreshTokenModel = new RefreshTokenModel("test", new DateTime());

    _tokenHelperMock
      .Setup(e => e.GenerateRefreshToken())
      .Returns(tRefreshTokenModel);

    _userManagerMock
      .Setup(e => e.Update(It.IsAny<User>()))
      .ReturnsAsync(IdentityResult.Success);

    var actual = await _authenticationHelper.GenerateAndSaveRefreshToken(tUser);

    actual.Should().BeEquivalentTo(tRefreshTokenModel.Token);

    _tokenHelperMock.Verify(e => e.GenerateRefreshToken());

    _userManagerMock.Verify(e => e.Update(tUser));
    tUser.RefreshToken.Should().NotBeEmpty();
    tUser.RefreshTokenExpireDate.Should().NotBeNull();

    _userManagerMock.VerifyNoOtherCalls();
    _roleManagerMock.VerifyNoOtherCalls();
    _tokenHelperMock.VerifyNoOtherCalls();
  }
  
  [Fact]
  public async Task GenerateRefreshToken_CannotUpdateUser_ThrowInternal()
  {
    var tUser = new User { Id = Guid.NewGuid() };

    var tRefreshTokenModel = new RefreshTokenModel("test", new DateTime());

    _tokenHelperMock
      .Setup(e => e.GenerateRefreshToken())
      .Returns(tRefreshTokenModel);

    _userManagerMock
      .Setup(e => e.Update(It.IsAny<User>()))
      .ReturnsAsync(IdentityResult.Failed());

    var action = () => _authenticationHelper.GenerateAndSaveRefreshToken(tUser);

    await action.Should().ThrowAsync<InternalException>();
    
    _tokenHelperMock.Verify(e => e.GenerateRefreshToken());

    _userManagerMock.Verify(e => e.Update(tUser));
    tUser.RefreshToken.Should().NotBeEmpty();
    tUser.RefreshTokenExpireDate.Should().NotBeNull();

    _userManagerMock.VerifyNoOtherCalls();
    _roleManagerMock.VerifyNoOtherCalls();
    _tokenHelperMock.VerifyNoOtherCalls();
  }
}