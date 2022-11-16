using System;
using System.Collections.Generic;
using System.Linq;
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

  private readonly AuthenticationService _authenticationService;

  public AuthenticationServiceTest()
  {
    _authenticationService = new AuthenticationService(
      _userManagerMock.Object,
      _authenticationManagerMock.Object
    );
  }

  [Fact]
  public async Task Login_ValidInput_ReturnCorrectResult()
  {
    var tLoginDto = new LoginDto { Email = "tEmail", Password = "tPass" };
    var tGuid = Guid.NewGuid();
    var tUser = new User { Id = tGuid, Email = tLoginDto.Email, UserName = "tName", EmailConfirmed = true};

    _userManagerMock
      .Setup(e => e.FindByEmail(It.IsAny<string>()))
      .ReturnsAsync(tUser);

    _authenticationManagerMock
      .Setup(e => e.SignInWithPassword(It.IsAny<User>(), It.IsAny<string>()))
      .ReturnsAsync(SignInResult.Success);

    var actual = await _authenticationService.Login(tLoginDto);
    actual.Email.Should().BeEquivalentTo(tUser.Email);
    actual.Id.Should().Be(tUser.Id);
    actual.EmailConfirmed.Should().BeTrue();
    actual.UserName.Should().BeEquivalentTo(tUser.UserName);

    _userManagerMock.Verify(e => e.FindByEmail(tLoginDto.Email));
    _authenticationManagerMock.Verify(e => e.SignInWithPassword(tUser, tLoginDto.Password));

    _userManagerMock.VerifyNoOtherCalls();
    _authenticationManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Login_EmailDoNotExists_ThrowUnauthorized()
  {
    var tLoginDto = new LoginDto { Email = "tEmail", Password = "tPass" };

    _userManagerMock
      .Setup(e => e.FindByEmail(It.IsAny<string>()))
      .ReturnsAsync(() => null);

    var action = () => _authenticationService.Login(tLoginDto);

    await action.Should().ThrowAsync<UnauthorizedException>();

    _userManagerMock.Verify(e => e.FindByEmail(tLoginDto.Email));

    _userManagerMock.VerifyNoOtherCalls();
    _authenticationManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Login_SignInNotSucceed_ThrowUnauthorized()
  {
    var tLoginDto = new LoginDto { Email = "tEmail", Password = "tPass" };
    var tUser = new User() { EmailConfirmed = true };

    _userManagerMock
      .Setup(e => e.FindByEmail(It.IsAny<string>()))
      .ReturnsAsync(tUser);

    _authenticationManagerMock
      .Setup(e => e.SignInWithPassword(It.IsAny<User>(), It.IsAny<string>()))
      .ReturnsAsync(SignInResult.Failed);

    var action = () => _authenticationService.Login(tLoginDto);

    await action.Should().ThrowAsync<UnauthorizedException>();

    _userManagerMock.Verify(e => e.FindByEmail(tLoginDto.Email));
    _authenticationManagerMock.Verify(e => e.SignInWithPassword(tUser, tLoginDto.Password));

    _userManagerMock.VerifyNoOtherCalls();
    _authenticationManagerMock.VerifyNoOtherCalls();
  }
  
  [Fact]
  public async Task Logout_WithSuccess()
  {
    _authenticationManagerMock.Setup(auth => auth.SignOut())
      .Returns(Task.CompletedTask);
    await _authenticationService.Logout();
    _authenticationManagerMock.Verify(auth => auth.SignOut());
  }

}
