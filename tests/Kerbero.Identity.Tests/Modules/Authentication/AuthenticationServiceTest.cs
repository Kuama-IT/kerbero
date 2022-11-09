using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kerbero.Identity.Common.Exceptions;
using Kerbero.Identity.Library.Modules.Authentication.Dtos;
using Kerbero.Identity.Modules.Authentication.Services;
using Kerbero.Identity.Modules.Users.Entities;
using Kerbero.Identity.Modules.Users.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Kerbero.Identity.Tests.Modules.Authentication;

public class AuthenticationServiceTest
{
  private readonly Mock<IUserManager> _userManagerMock = new();
  private readonly Mock<IAuthenticationManager> _authenticationManagerMock = new();
  private readonly Mock<IAuthenticationHelper> _authenticationHelper = new();

  private readonly AuthenticationService _authenticationService;

  public AuthenticationServiceTest()
  {
    _authenticationService = new AuthenticationService(
      _userManagerMock.Object,
      _authenticationManagerMock.Object,
      _authenticationHelper.Object
    );
  }

  [Fact]
  public async Task LoginWithEmail_ValidInput_ReturnCorrectResult()
  {
    var tLoginDto = new LoginEmailDto { Email = "tEmail", Password = "tPass" };
    var tUser = new User { Id = Guid.NewGuid() };

    var tAccessToken = "jwt";
    var tRefreshToken = "refresh token";

    _userManagerMock
      .Setup(e => e.FindByEmail(It.IsAny<string>()))
      .ReturnsAsync(tUser);

    _authenticationManagerMock
      .Setup(e => e.SignInWithPassword(It.IsAny<User>(), It.IsAny<string>()))
      .ReturnsAsync(SignInResult.Success);

    _authenticationHelper
      .Setup(e => e.GenerateAccessToken(It.IsAny<User>()))
      .ReturnsAsync(tAccessToken);

    _authenticationHelper
      .Setup(e => e.GenerateAndSaveRefreshToken(It.IsAny<User>()))
      .ReturnsAsync(tRefreshToken);


    var expected = new AuthenticatedDto
    {
      AccessToken = tAccessToken,
      RefreshToken = tRefreshToken,
    };

    var actual = await _authenticationService.LoginWithEmail(tLoginDto);

    actual.Should().BeEquivalentTo(expected);

    _userManagerMock.Verify(e => e.FindByEmail(tLoginDto.Email));
    _authenticationManagerMock.Verify(e => e.SignInWithPassword(tUser, tLoginDto.Password));

    _authenticationHelper.Verify(e => e.GenerateAccessToken(tUser));
    _authenticationHelper.Verify(e => e.GenerateAndSaveRefreshToken(tUser));

    _userManagerMock.VerifyNoOtherCalls();
    _authenticationManagerMock.VerifyNoOtherCalls();
    _authenticationHelper.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task LoginWithEmail_EmailDoNotExists_ThrowUnauthorized()
  {
    var tLoginDto = new LoginEmailDto { Email = "tEmail", Password = "tPass" };

    _userManagerMock
      .Setup(e => e.FindByEmail(It.IsAny<string>()))
      .ReturnsAsync(() => null);

    var action = () => _authenticationService.LoginWithEmail(tLoginDto);

    await action.Should().ThrowAsync<UnauthorizedException>();

    _userManagerMock.Verify(e => e.FindByEmail(tLoginDto.Email));

    _userManagerMock.VerifyNoOtherCalls();
    _authenticationManagerMock.VerifyNoOtherCalls();
    _authenticationHelper.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task LoginWithEmail_SignInNotSucceed_ThrowUnauthorized()
  {
    var tLoginDto = new LoginEmailDto { Email = "tEmail", Password = "tPass" };
    var tUser = new User();

    _userManagerMock
      .Setup(e => e.FindByEmail(It.IsAny<string>()))
      .ReturnsAsync(tUser);

    _authenticationManagerMock
      .Setup(e => e.SignInWithPassword(It.IsAny<User>(), It.IsAny<string>()))
      .ReturnsAsync(SignInResult.Failed);

    var action = () => _authenticationService.LoginWithEmail(tLoginDto);

    await action.Should().ThrowAsync<UnauthorizedException>();

    _userManagerMock.Verify(e => e.FindByEmail(tLoginDto.Email));
    _authenticationManagerMock.Verify(e => e.SignInWithPassword(tUser, tLoginDto.Password));

    _userManagerMock.VerifyNoOtherCalls();
    _authenticationManagerMock.VerifyNoOtherCalls();
    _authenticationHelper.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task LoginWithRefreshToken_ValidInput_ReturnCorrectResult()
  {
    var tLoginRefreshTokenDto = new LoginRefreshTokenDto();

    var tUser = new User();

    var tAccessToken = "jwt";
    var tRefreshToken = "refresh token";

    _userManagerMock
      .Setup(e => e.FindByRefreshToken(It.IsAny<string>()))
      .ReturnsAsync(tUser);
    
    _authenticationHelper
      .Setup(e => e.GenerateAccessToken(It.IsAny<User>()))
      .ReturnsAsync(tAccessToken);

    _authenticationHelper
      .Setup(e => e.GenerateAndSaveRefreshToken(It.IsAny<User>()))
      .ReturnsAsync(tRefreshToken);


    var expected = new AuthenticatedDto
    {
      AccessToken = tAccessToken,
      RefreshToken = tRefreshToken,
    };

    var actual = await _authenticationService.LoginWithRefreshToken(tLoginRefreshTokenDto);

    actual.Should().BeEquivalentTo(expected);

    _userManagerMock.Verify(e => e.FindByRefreshToken(tLoginRefreshTokenDto.Token));

    _authenticationHelper.Verify(e => e.GenerateAccessToken(tUser));
    _authenticationHelper.Verify(e => e.GenerateAndSaveRefreshToken(tUser));

    _userManagerMock.VerifyNoOtherCalls();
    _authenticationManagerMock.VerifyNoOtherCalls();
    _authenticationHelper.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task LoginWithRefreshToken_RefreshTokenNotExists_ThrowUnauthorized()
  {
    var tLoginRefreshTokenDto = new LoginRefreshTokenDto();

    _userManagerMock
      .Setup(e => e.FindByEmail(It.IsAny<string>()))
      .ReturnsAsync(() => null);

    var action = () => _authenticationService.LoginWithRefreshToken(tLoginRefreshTokenDto);

    await action.Should().ThrowAsync<UnauthorizedException>();

    _userManagerMock.Verify(e => e.FindByRefreshToken(tLoginRefreshTokenDto.Token));

    _userManagerMock.VerifyNoOtherCalls();
    _authenticationManagerMock.VerifyNoOtherCalls();
    _authenticationHelper.VerifyNoOtherCalls();
  }
  
  [Fact]
  public async Task LoginWithRefreshToken_ExpiredRefreshToken_ThrowUnauthorized()
  {
    var tLoginRefreshTokenDto = new LoginRefreshTokenDto();

    var tUser = new User
    {
      RefreshTokenExpireDate = DateTime.UtcNow.AddDays(-1),
    };
    
    _userManagerMock
      .Setup(e => e.FindByRefreshToken(It.IsAny<string>()))
      .ReturnsAsync(tUser);
    
    var action = () =>_authenticationService.LoginWithRefreshToken(tLoginRefreshTokenDto);

    await action.Should().ThrowAsync<UnauthorizedException>();

    _userManagerMock.Verify(e => e.FindByRefreshToken(tLoginRefreshTokenDto.Token));

    _userManagerMock.VerifyNoOtherCalls();
    _authenticationManagerMock.VerifyNoOtherCalls();
    _authenticationHelper.VerifyNoOtherCalls();
  }
}