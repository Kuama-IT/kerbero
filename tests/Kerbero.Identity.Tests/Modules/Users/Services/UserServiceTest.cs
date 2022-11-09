using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Kerbero.Identity.Common.Exceptions;
using Kerbero.Identity.Library.Modules.Claims.Dtos;
using Kerbero.Identity.Library.Modules.Users.Dtos;
using Kerbero.Identity.Modules.Claims.Mappings;
using Kerbero.Identity.Modules.Claims.Utils;
using Kerbero.Identity.Modules.Notifier.Events;
using Kerbero.Identity.Modules.Notifier.Services;
using Kerbero.Identity.Modules.Roles.Entities;
using Kerbero.Identity.Modules.Roles.Services;
using Kerbero.Identity.Modules.Users.Entities;
using Kerbero.Identity.Modules.Users.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Kerbero.Identity.Tests.Modules.Users.Services;

public class UserServiceTest
{
  private readonly Mock<IKerberoIdentityNotifier> _kerberoIdentityNotifierMock = new();

  private readonly Mock<IUserManager> _userManagerMock = new();
  private readonly Mock<IRoleManager> _roleManagerMock = new();
  private readonly Mock<IValidator<UserCreateDto>> _userCreateDtoValidatorMock = new();
  private readonly Mock<IValidator<UserUpdateDto>> _userUpdateDtoValidatorMock = new();

  private readonly UserService _userService;

  public UserServiceTest()
  {
    _userService = new UserService(
      _kerberoIdentityNotifierMock.Object,
      _userManagerMock.Object,
      _roleManagerMock.Object,
      _userCreateDtoValidatorMock.Object,
      _userUpdateDtoValidatorMock.Object
    );
  }

  [Fact]
  public async Task GetAll_ReturnCorrectResult()
  {
    var entities = new List<User>
    {
      new User(),
    };

    _userManagerMock
      .Setup(e => e.GetAll())
      .ReturnsAsync(entities);

    var actual = await _userService.GetAll();

    actual.Should().AllBeOfType<UserReadDto>();
    actual.Count.Should().Be(1);
  }

  [Fact]
  public async Task GetById_Exists_ReturnCorrectResult()
  {
    var tId = Guid.NewGuid();
    var entity = new User();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(entity);

    var actual = await _userService.GetById(tId);

    actual.Should().BeOfType<UserReadDto>();

    _userManagerMock.Verify(e => e.GetById(tId));
  }

  [Fact]
  public async Task GetById_NotExists_ThrowNotFound()
  {
    var tId = Guid.NewGuid();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var action = () => _userService.GetById(tId);

    await action.Should().ThrowAsync<NotFoundException>();

    _userManagerMock.Verify(e => e.GetById(tId));
  }

  [Fact]
  public async Task GetClaimsById_Exists_ReturnCorrectResult()
  {
    var tId = Guid.NewGuid();
    var tUser = new User();
    var tClaims = new List<Claim>();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.GetClaimsByUser(It.IsAny<User>()))
      .ReturnsAsync(tClaims);

    var actual = await _userService.GetClaimsById(tId);

    actual.Should().AllBeOfType<ClaimReadDto>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.Verify(e => e.GetClaimsByUser(tUser));
  }

  [Fact]
  public async Task GetClaimsById_NotExists_ThrowNotFound()
  {
    var tId = Guid.NewGuid();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var action = () => _userService.GetClaimsById(tId);

    await action.Should().ThrowAsync<NotFoundException>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Create_ValidInput_ReturnCorrectResult()
  {
    var tCreateDto = new UserCreateDto();
    var tPassedUser = new User();
    var tResult = IdentityResult.Success;

    _userCreateDtoValidatorMock
      .Setup(e => e.Validate(It.IsAny<ValidationContext<UserCreateDto>>()))
      .Returns(new ValidationResult());

    _userManagerMock
      .Setup(e => e.Create(It.IsAny<User>(), It.IsAny<string>()))
      .Callback<User, string>((user, _) => { tPassedUser = user; })
      .ReturnsAsync(tResult);

    _kerberoIdentityNotifierMock
      .Setup(e => e.EmitUserCreateEvent(It.IsAny<UserCreateEvent>()));

    var actual = await _userService.Create(tCreateDto);

    actual.Should().BeOfType<UserReadDto>();

    _userCreateDtoValidatorMock.VerifyAll();
    _userManagerMock.Verify(e => e.Create(tPassedUser, tCreateDto.Password));
    _kerberoIdentityNotifierMock.Verify(e => e.EmitUserCreateEvent(new UserCreateEvent(tPassedUser.Id)));
    
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
    _userCreateDtoValidatorMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Create_ValidInput_NotSucceed_ThrowBadRequest()
  {
    var tCreateDto = new UserCreateDto();
    var tResult = IdentityResult.Failed();
    var tUserPassed = new User();

    _userCreateDtoValidatorMock
      .Setup(e => e.Validate(It.IsAny<ValidationContext<UserCreateDto>>()));

    _userManagerMock
      .Setup(e => e.Create(It.IsAny<User>(), It.IsAny<string>()))
      .Callback<User,string>((user, password) => { tUserPassed = user;})
      .ReturnsAsync(tResult);

    var action = () => _userService.Create(tCreateDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _userCreateDtoValidatorMock.VerifyAll();
    _userManagerMock.Verify(e => e.Create(tUserPassed,tCreateDto.Password));
    
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
    _userCreateDtoValidatorMock.VerifyNoOtherCalls();

  }

  [Fact]
  public async Task Update_ValidInput_ReturnCorrectResult()
  {
    var tId = Guid.NewGuid();
    var tUser = new User();
    var tResult = IdentityResult.Success;

    var tUpdateDto = new UserUpdateDto();

    _userUpdateDtoValidatorMock
      .Setup(e => e.Validate(It.IsAny<ValidationContext<UserUpdateDto>>()))
      .Returns(new ValidationResult());
    
    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.Update(It.IsAny<User>()))
      .ReturnsAsync(tResult);

    _kerberoIdentityNotifierMock
      .Setup(e => e.EmitUserUpdateEvent(It.IsAny<UserUpdateEvent>()));

    var actual = await _userService.Update(tId, tUpdateDto);

    actual.Should().BeOfType<UserReadDto>();

    _userUpdateDtoValidatorMock.VerifyAll();
    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.Verify(e => e.Update(tUser));
    _kerberoIdentityNotifierMock.Verify(e => e.EmitUserUpdateEvent(new UserUpdateEvent(tUser.Id)));
  }

  [Fact]
  public async Task Update_NoSucceed_ThrowBadRequest()
  {
    var tId = Guid.NewGuid();

    var tUpdateDto = new UserUpdateDto();

    _userUpdateDtoValidatorMock
      .Setup(e => e.Validate(It.IsAny<ValidationContext<UserUpdateDto>>()))
      .Returns(new ValidationResult());
    
    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var action = () => _userService.Update(tId, tUpdateDto);

    await action.Should().ThrowAsync<NotFoundException>();
    
    _userUpdateDtoValidatorMock.VerifyAll();
    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Update_NotExists_ThrowNotFound()
  {
    var tId = Guid.NewGuid();
    var tUser = new User();
    var tResult = IdentityResult.Failed();

    var tUpdateDto = new UserUpdateDto();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.Update(It.IsAny<User>()))
      .ReturnsAsync(tResult);

    var action = () => _userService.Update(tId, tUpdateDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.Verify(e => e.Update(tUser));
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Delete_ValidInput_ReturnCorrectResult()
  {
    var tId = Guid.NewGuid();
    var tUser = new User();
    var tResult = IdentityResult.Success;

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.Delete(It.IsAny<User>()))
      .ReturnsAsync(tResult);

    _kerberoIdentityNotifierMock
      .Setup(e => e.EmitUserDeleteEvent(It.IsAny<UserDeleteEvent>()));

    var actual = await _userService.Delete(tId);

    actual.Should().BeOfType<UserReadDto>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.Verify(e => e.Delete(tUser));
    _kerberoIdentityNotifierMock.Verify(e => e.EmitUserDeleteEvent(new UserDeleteEvent(tUser.Id)));
  }

  [Fact]
  public async Task Delete_NoSucceed_ThrowBadRequest()
  {
    var tId = Guid.NewGuid();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var action = () => _userService.Delete(tId);

    await action.Should().ThrowAsync<NotFoundException>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Delete_NotExists_ThrowNotFound()
  {
    var tId = Guid.NewGuid();
    var tUser = new User();
    var tResult = IdentityResult.Failed();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.Delete(It.IsAny<User>()))
      .ReturnsAsync(tResult);

    var action = () => _userService.Delete(tId);

    await action.Should().ThrowAsync<BadRequestException>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.Verify(e => e.Delete(tUser));
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task SetRoles_ValidInput_ReturnCorrectResult()
  {
    var tId = Guid.NewGuid();
    var tSetRolesDto = new UserSetRolesDto
    {
      RoleNames = new List<string> { "roleToAdd" }
    };

    var tUser = new User();
    var tUserRolesNames = new List<string> { "roleToRemove" };

    var tResultRemoveRoles = IdentityResult.Success;
    var tResultAddRoles = IdentityResult.Success;

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.GetRolesNamesByUser(It.IsAny<User>()))
      .ReturnsAsync(tUserRolesNames);

    _userManagerMock
      .Setup(e => e.RemoveRolesByNamesFromUser(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
      .ReturnsAsync(tResultRemoveRoles);

    _userManagerMock
      .Setup(e => e.AddRolesByNamesToUser(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
      .ReturnsAsync(tResultAddRoles);

    await _userService.SetRoles(tId, tSetRolesDto);

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.Verify(e => e.GetRolesNamesByUser(tUser));
    _userManagerMock.Verify(e => e.RemoveRolesByNamesFromUser(tUser, tUserRolesNames));
    _userManagerMock.Verify(e => e.AddRolesByNamesToUser(tUser, tSetRolesDto.RoleNames));
  }

  [Fact]
  public async Task SetRoles_NotExists_ThrowNotFound()
  {
    var tId = Guid.NewGuid();
    var tSetRolesDto = new UserSetRolesDto
    {
      RoleNames = new List<string> { "roleToAdd" }
    };

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var action = () => _userService.SetRoles(tId, tSetRolesDto);

    await action.Should().ThrowAsync<NotFoundException>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task SetRoles_FailedRemove_ReturnCorrectResult()
  {
    var tId = Guid.NewGuid();
    var tSetRolesDto = new UserSetRolesDto
    {
      RoleNames = new List<string> { "roleToAdd" }
    };

    var tUser = new User();
    var tUserRolesNames = new List<string> { "roleToRemove" };

    var tResultRemoveRoles = IdentityResult.Failed();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.GetRolesNamesByUser(It.IsAny<User>()))
      .ReturnsAsync(tUserRolesNames);

    _userManagerMock
      .Setup(e => e.RemoveRolesByNamesFromUser(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
      .ReturnsAsync(tResultRemoveRoles);

    var action = () => _userService.SetRoles(tId, tSetRolesDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.Verify(e => e.GetRolesNamesByUser(tUser));
    _userManagerMock.Verify(e => e.RemoveRolesByNamesFromUser(tUser, tUserRolesNames));
    _userManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task SetRoles_FailedAdd_ReturnCorrectResult()
  {
    var tId = Guid.NewGuid();
    var tSetRolesDto = new UserSetRolesDto
    {
      RoleNames = new List<string> { "roleToAdd" }
    };

    var tUser = new User();
    var tUserRolesNames = new List<string> { "roleToRemove" };

    var tResultRemoveRoles = IdentityResult.Success;
    var tResultAddRoles = IdentityResult.Failed();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.GetRolesNamesByUser(It.IsAny<User>()))
      .ReturnsAsync(tUserRolesNames);

    _userManagerMock
      .Setup(e => e.RemoveRolesByNamesFromUser(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
      .ReturnsAsync(tResultRemoveRoles);

    _userManagerMock
      .Setup(e => e.AddRolesByNamesToUser(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
      .ReturnsAsync(tResultAddRoles);

    var action = () => _userService.SetRoles(tId, tSetRolesDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.Verify(e => e.GetRolesNamesByUser(tUser));
    _userManagerMock.Verify(e => e.RemoveRolesByNamesFromUser(tUser, tUserRolesNames));
    _userManagerMock.Verify(e => e.AddRolesByNamesToUser(tUser, tSetRolesDto.RoleNames));
    _userManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task SetClaims_ValidInput_ReturnCorrectResult()
  {
    var tSetClaimsDto = new UserSetClaimsDto()
    {
      Claims = new List<ClaimCreateDto>(),
    };

    var tId = Guid.NewGuid();
    var tUser = new User();
    var tUserClaims = new List<Claim>
    {
      new Claim("key", "val")
    };

    var tClaimsRemoveResult = IdentityResult.Success;

    var tClaimsAddResult = IdentityResult.Success;
    var tClaimsToAdd = ClaimMappings.Map(tSetClaimsDto.Claims);

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.GetClaimsByUser(It.IsAny<User>()))
      .ReturnsAsync(tUserClaims);

    _userManagerMock
      .Setup(e => e.RemoveClaimsToUser(It.IsAny<User>(), It.IsAny<IEnumerable<Claim>>()))
      .ReturnsAsync(tClaimsRemoveResult);

    _userManagerMock
      .Setup(e => e.AddClaimsToUser(It.IsAny<User>(), It.IsAny<IEnumerable<Claim>>()))
      .ReturnsAsync(tClaimsAddResult);

    await _userService.SetClaims(tId, tSetClaimsDto);

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.Verify(e => e.GetClaimsByUser(tUser));
    _userManagerMock.Verify(e => e.RemoveClaimsToUser(tUser, tUserClaims));
    _userManagerMock.Verify(e => e.AddClaimsToUser(tUser, tClaimsToAdd));
    _userManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task SetClaims_NotExists_ThrowNotFound()
  {
    var tSetClaimsDto = new UserSetClaimsDto()
    {
      Claims = new List<ClaimCreateDto>(),
    };

    var tId = Guid.NewGuid();
    var tUser = new User();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var action = () => _userService.SetClaims(tId, tSetClaimsDto);

    await action.Should().ThrowAsync<NotFoundException>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task SetClaims_CannotRemove_ThrowBadRequest()
  {
    var tSetClaimsDto = new UserSetClaimsDto()
    {
      Claims = new List<ClaimCreateDto>(),
    };

    var tId = Guid.NewGuid();
    var tUser = new User();
    var tUserClaims = new List<Claim>
    {
      new Claim("key", "val")
    };

    var tClaimsRemoveResult = IdentityResult.Failed();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.GetClaimsByUser(It.IsAny<User>()))
      .ReturnsAsync(tUserClaims);

    _userManagerMock
      .Setup(e => e.RemoveClaimsToUser(It.IsAny<User>(), It.IsAny<IEnumerable<Claim>>()))
      .ReturnsAsync(tClaimsRemoveResult);

    var action = () => _userService.SetClaims(tId, tSetClaimsDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.Verify(e => e.GetClaimsByUser(tUser));
    _userManagerMock.Verify(e => e.RemoveClaimsToUser(tUser, tUserClaims));
    _userManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task SetClaims_CannotAdd_ThrowBadRequest()
  {
    var tSetClaimsDto = new UserSetClaimsDto()
    {
      Claims = new List<ClaimCreateDto>(),
    };

    var tId = Guid.NewGuid();
    var tUser = new User();
    var tUserClaims = new List<Claim>
    {
      new Claim("key", "val")
    };

    var tClaimsRemoveResult = IdentityResult.Success;

    var tClaimsAddResult = IdentityResult.Failed();
    var tClaimsToAdd = ClaimMappings.Map(tSetClaimsDto.Claims);

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.GetClaimsByUser(It.IsAny<User>()))
      .ReturnsAsync(tUserClaims);

    _userManagerMock
      .Setup(e => e.RemoveClaimsToUser(It.IsAny<User>(), It.IsAny<IEnumerable<Claim>>()))
      .ReturnsAsync(tClaimsRemoveResult);


    _userManagerMock
      .Setup(e => e.AddClaimsToUser(It.IsAny<User>(), It.IsAny<IEnumerable<Claim>>()))
      .ReturnsAsync(tClaimsAddResult);

    var action = () => _userService.SetClaims(tId, tSetClaimsDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _userManagerMock.Verify(e => e.GetById(tId));
    _userManagerMock.Verify(e => e.GetClaimsByUser(tUser));
    _userManagerMock.Verify(e => e.RemoveClaimsToUser(tUser, tUserClaims));
    _userManagerMock.Verify(e => e.AddClaimsToUser(tUser, tClaimsToAdd));
    _userManagerMock.VerifyNoOtherCalls();
  }


  [Fact]
  public async Task CreateAdmin_CannotCreateUser_ThrowBadRequest()
  {
    var tCreateDto = new UserCreateDto { Password = "tpas" };
    var tUser = new User();
    var tCreateResult = IdentityResult.Failed();

    _userManagerMock
      .Setup(e => e.Create(It.IsAny<User>(), It.IsAny<string>()))
      .Callback<User, string>((user, _) => { tUser = user; })
      .ReturnsAsync(tCreateResult);

    var action = () => _userService.CreateAdmin(tCreateDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _userManagerMock.Verify(e => e.Create(tUser, tCreateDto.Password));
    _userManagerMock.VerifyNoOtherCalls();
    _roleManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task CreateAdmin_NoAdminRole_ReturnCorrectResult()
  {
    var tCreateDto = new UserCreateDto { Password = "tpas" };
    var tUser = new User();
    var tRole = new Role();

    _userManagerMock
      .Setup(e => e.Create(It.IsAny<User>(), It.IsAny<string>()))
      .Callback<User, string>((user, _) => { tUser = user; })
      .ReturnsAsync(IdentityResult.Success);

    _roleManagerMock
      .Setup(e => e.FindByNameAsync(It.IsAny<string>()))
      .ReturnsAsync(() => null);

    _roleManagerMock
      .Setup(e => e.Create(It.IsAny<Role>()))
      .Callback<Role>(role => tRole = role)
      .ReturnsAsync(IdentityResult.Success);

    _roleManagerMock
      .Setup(e => e.AddClaimToRole(It.IsAny<Role>(), It.IsAny<Claim>()))
      .ReturnsAsync(IdentityResult.Success);

    _userManagerMock
      .Setup(e => e.AddRoleByNameToUser(It.IsAny<User>(), It.IsAny<string>()))
      .ReturnsAsync(IdentityResult.Success);

    var actual = await _userService.CreateAdmin(tCreateDto);

    actual.Should().BeOfType<UserReadDto>();

    _userManagerMock.Verify(e => e.Create(tUser, tCreateDto.Password));
    _roleManagerMock.Verify(e => e.FindByNameAsync("Admin"));
    _roleManagerMock.Verify(e => e.Create(tRole));
    _roleManagerMock.Verify(e => e.AddClaimToRole(tRole, It.IsAny<Claim>()),
      Times.Exactly(ClaimsDefinition.KuIdentityClaims.Count));
    _userManagerMock.Verify(e => e.AddRoleByNameToUser(tUser, "Admin"));

    _userManagerMock.VerifyNoOtherCalls();
    _roleManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task CreateAdmin_NoAdminRole_CannotCreateRole_ThrowBadRequest()
  {
    var tCreateDto = new UserCreateDto { Password = "tpas" };
    var tUser = new User();
    var tRole = new Role();

    _userManagerMock
      .Setup(e => e.Create(It.IsAny<User>(), It.IsAny<string>()))
      .Callback<User, string>((user, _) => { tUser = user; })
      .ReturnsAsync(IdentityResult.Success);

    _roleManagerMock
      .Setup(e => e.FindByNameAsync(It.IsAny<string>()))
      .ReturnsAsync(() => null);

    _roleManagerMock
      .Setup(e => e.Create(It.IsAny<Role>()))
      .Callback<Role>(role => tRole = role)
      .ReturnsAsync(IdentityResult.Failed());

    var action = () => _userService.CreateAdmin(tCreateDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _userManagerMock.Verify(e => e.Create(tUser, tCreateDto.Password));
    _roleManagerMock.Verify(e => e.FindByNameAsync("Admin"));
    _roleManagerMock.Verify(e => e.Create(tRole));

    _userManagerMock.VerifyNoOtherCalls();
    _roleManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task CreateAdmin_NoAdminRole_CannotAddClaim_ThrowBadRequest()
  {
    var tCreateDto = new UserCreateDto { Password = "tpas" };
    var tUser = new User();
    var tRole = new Role();

    _userManagerMock
      .Setup(e => e.Create(It.IsAny<User>(), It.IsAny<string>()))
      .Callback<User, string>((user, _) => { tUser = user; })
      .ReturnsAsync(IdentityResult.Success);

    _roleManagerMock
      .Setup(e => e.FindByNameAsync(It.IsAny<string>()))
      .ReturnsAsync(() => null);

    _roleManagerMock
      .Setup(e => e.Create(It.IsAny<Role>()))
      .Callback<Role>(role => tRole = role)
      .ReturnsAsync(IdentityResult.Success);


    _roleManagerMock
      .Setup(e => e.AddClaimToRole(It.IsAny<Role>(), It.IsAny<Claim>()))
      .ReturnsAsync(IdentityResult.Failed());

    var action = () => _userService.CreateAdmin(tCreateDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _userManagerMock.Verify(e => e.Create(tUser, tCreateDto.Password));
    _roleManagerMock.Verify(e => e.FindByNameAsync("Admin"));
    _roleManagerMock.Verify(e => e.Create(tRole));
    _roleManagerMock.Verify(e => e.AddClaimToRole(tRole, It.IsAny<Claim>()));

    _userManagerMock.VerifyNoOtherCalls();
    _roleManagerMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task CreateAdmin_CannotAddRoleToUser_ThrowBadRequest()
  {
    var tCreateDto = new UserCreateDto { Password = "tpas" };
    var tUser = new User();
    var tRole = new Role { Name = "Admin" };

    _userManagerMock
      .Setup(e => e.Create(It.IsAny<User>(), It.IsAny<string>()))
      .Callback<User, string>((user, _) => { tUser = user; })
      .ReturnsAsync(IdentityResult.Success);

    _roleManagerMock
      .Setup(e => e.FindByNameAsync(It.IsAny<string>()))
      .ReturnsAsync(tRole);

    _userManagerMock
      .Setup(e => e.AddRoleByNameToUser(It.IsAny<User>(), It.IsAny<string>()))
      .ReturnsAsync(IdentityResult.Failed());

    var action = () => _userService.CreateAdmin(tCreateDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _userManagerMock.Verify(e => e.Create(tUser, tCreateDto.Password));
    _roleManagerMock.Verify(e => e.FindByNameAsync("Admin"));
    _userManagerMock.Verify(e => e.AddRoleByNameToUser(tUser, "Admin"));

    _userManagerMock.VerifyNoOtherCalls();
    _roleManagerMock.VerifyNoOtherCalls();
  }
}