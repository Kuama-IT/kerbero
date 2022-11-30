using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Kerbero.Identity.Common.Exceptions;
using Kerbero.Identity.Library.Modules.Claims.Dtos;
using Kerbero.Identity.Library.Modules.Roles.Dtos;
using Kerbero.Identity.Modules.Notifier.Events;
using Kerbero.Identity.Modules.Notifier.Services;
using Kerbero.Identity.Modules.Roles.Entities;
using Kerbero.Identity.Modules.Roles.Mappings;
using Kerbero.Identity.Modules.Roles.Services;
using Kerbero.Identity.Modules.Users.Entities;
using Kerbero.Identity.Modules.Users.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Kerbero.Identity.Tests.Modules.Roles.Services;

public class RoleServiceTest
{
  private readonly Mock<IRoleManager> _roleManagerMock = new();
  private readonly Mock<IUserManager> _userManagerMock = new();
  private readonly Mock<IKerberoIdentityNotifier> _kerberoIdentityNotifierMock = new();

  private readonly RoleService _roleService;

  public RoleServiceTest()
  {
    _roleService = new RoleService(
      _roleManagerMock.Object,
      _userManagerMock.Object,
      _kerberoIdentityNotifierMock.Object
    );
  }

  [Fact]
  public async Task GetAll_ReturnCorrectResult()
  {
    var tRoles = new List<Role>
    {
      new Role { Name = "test" },
    };

    _roleManagerMock
      .Setup(e => e.GetAll())
      .ReturnsAsync(tRoles);

    var actual = await _roleService.GetAll();
    actual.Should().AllBeOfType<RoleReadDto>();
    actual.Count.Should().Be(1);

    _roleManagerMock.Verify(e => e.GetAll());

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task GetById_Exists_ReturnCorrectResult()
  {
    var tRole = new Role { Id = Guid.NewGuid(), Name = "test" };

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tRole);

    var actual = await _roleService.GetById(tRole.Id);

    var expected = RoleMappings.Map(tRole);
    actual.Should().BeEquivalentTo(expected);

    _roleManagerMock.Verify(e => e.GetById(tRole.Id));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task GetById_NotExists_ThrowNotFound()
  {
    var tRole = new Role { Id = Guid.NewGuid() };

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var actual = () => _roleService.GetById(tRole.Id);

    await actual.Should().ThrowAsync<NotFoundException>();

    _roleManagerMock.Verify(e => e.GetById(tRole.Id));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task GetByUserId_Exists_ReturnCorrectResult()
  {
    var tUserId = Guid.NewGuid();
    var tUser = new User { Id = tUserId };
    var tRoles = new List<Role>
    {
      new Role() { Name = "testRoleName" },
    };
    var tRolesNames = new List<string> { "testRoleName" };

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tUser);

    _userManagerMock
      .Setup(e => e.GetRolesNamesByUser(It.IsAny<User>()))
      .ReturnsAsync(tRolesNames);

    _roleManagerMock
      .Setup(e => e.GetByNames(It.IsAny<IEnumerable<string>>()))
      .ReturnsAsync(tRoles);


    var actual = await _roleService.GetByUserId(tUserId);

    actual.Should().AllBeOfType<RoleReadDto>();
    actual.Count.Should().Be(1);

    _userManagerMock.Verify(e => e.GetById(tUserId));
    _userManagerMock.Verify(e => e.GetRolesNamesByUser(tUser));
    _roleManagerMock.Verify(e => e.GetByNames(tRolesNames));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task GetByUserId_NotExists_ThrowNotFound()
  {
    var tUserId = Guid.NewGuid();

    _userManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var actual = () => _roleService.GetByUserId(tUserId);

    await actual.Should().ThrowAsync<NotFoundException>();

    _userManagerMock.Verify(e => e.GetById(tUserId));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task GetClaimsById_Exists_ReturnCorrectResult()
  {
    var tRoleId = Guid.NewGuid();
    var tRole = new Role { Id = tRoleId };
    var tClaims = new List<Claim>
    {
      new Claim("one", "val")
    };

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tRole);

    _roleManagerMock
      .Setup(e => e.GetClaimsByRole(It.IsAny<Role>()))
      .ReturnsAsync(tClaims);


    var actual = await _roleService.GetClaimsById(tRoleId);

    actual.Should().AllBeOfType<ClaimReadDto>();
    actual.Count.Should().Be(1);

    _roleManagerMock.Verify(e => e.GetById(tRoleId));
    _roleManagerMock.Verify(e => e.GetClaimsByRole(tRole));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task GetClaimsById_NotExists_ThrowNotFound()
  {
    var tRoleId = Guid.NewGuid();

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var actual = () => _roleService.GetClaimsById(tRoleId);

    await actual.Should().ThrowAsync<NotFoundException>();

    _roleManagerMock.Verify(e => e.GetById(tRoleId));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Create_ValidInput_ReturnCorrectResult()
  {
    var tCreateDto = new RoleCreateDto { Name = "test" };
    var tRole = new Role { Name = "test" };

    _roleManagerMock
      .Setup(e => e.Create(It.IsAny<Role>()))
      .Callback<Role>(e => tRole = e)
      .ReturnsAsync(IdentityResult.Success);

    _kerberoIdentityNotifierMock
      .Setup(e => e.EmitRoleCreateEvent(It.IsAny<RoleCreateEvent>()));

    var expected = RoleMappings.Map(tRole);

    var actual = await _roleService.Create(tCreateDto);

    actual.Should().BeEquivalentTo(expected);

    _roleManagerMock.Verify(e => e.Create(tRole));
    _kerberoIdentityNotifierMock.Verify(e => e.EmitRoleCreateEvent(new RoleCreateEvent(tRole.Id)));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Create_CannotCreate_ThrowBadRequest()
  {
    var tCreateDto = new RoleCreateDto();
    var tRole = new Role();

    _roleManagerMock
      .Setup(e => e.Create(It.IsAny<Role>()))
      .Callback<Role>(e => tRole = e)
      .ReturnsAsync(IdentityResult.Failed());


    var action = () => _roleService.Create(tCreateDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _roleManagerMock.Verify(e => e.Create(tRole));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Update_ValidInput_ReturnCorrectResult()
  {
    var tUpdateDto = new RoleUpdateDto { Name = "test" };
    var tRole = new Role { Id = Guid.NewGuid(), Name = "test" };

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tRole);

    _roleManagerMock
      .Setup(e => e.Update(It.IsAny<Role>()))
      .ReturnsAsync(IdentityResult.Success);

    _kerberoIdentityNotifierMock
      .Setup(e => e.EmitRoleUpdateEvent(It.IsAny<RoleUpdateEvent>()));

    var expected = RoleMappings.Map(tRole);

    var actual = await _roleService.Update(tRole.Id, tUpdateDto);

    actual.Should().BeEquivalentTo(expected);

    _roleManagerMock.Verify(e => e.GetById(tRole.Id));
    _roleManagerMock.Verify(e => e.Update(tRole));
    _kerberoIdentityNotifierMock.Verify(e => e.EmitRoleUpdateEvent(new RoleUpdateEvent(tRole.Id)));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Update_CannotUpdate_ThrowBadRequest()
  {
    var tUpdateDto = new RoleUpdateDto();
    var tRole = new Role { Id = Guid.NewGuid() };

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tRole);

    _roleManagerMock
      .Setup(e => e.Update(It.IsAny<Role>()))
      .ReturnsAsync(IdentityResult.Failed());

    var action = () => _roleService.Update(tRole.Id, tUpdateDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _roleManagerMock.Verify(e => e.GetById(tRole.Id));
    _roleManagerMock.Verify(e => e.Update(tRole));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Update_NotExists_ThrowNotFound()
  {
    var tUpdateDto = new RoleUpdateDto();
    var tRole = new Role { Id = Guid.NewGuid() };

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var action = () => _roleService.Update(tRole.Id, tUpdateDto);

    await action.Should().ThrowAsync<NotFoundException>();

    _roleManagerMock.Verify(e => e.GetById(tRole.Id));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Delete_ValidInput_ReturnCorrectResult()
  {
    var tRoleId = Guid.NewGuid();
    var tRole = new Role { Id = tRoleId, Name = "test" };

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tRole);

    _roleManagerMock
      .Setup(e => e.Delete(It.IsAny<Role>()))
      .ReturnsAsync(IdentityResult.Success);

    _kerberoIdentityNotifierMock
      .Setup(e => e.EmitRoleDeleteEvent(It.IsAny<RoleDeleteEvent>()));

    var expected = RoleMappings.Map(tRole);

    var actual = await _roleService.Delete(tRole.Id);

    actual.Should().BeEquivalentTo(expected);

    _roleManagerMock.Verify(e => e.GetById(tRole.Id));
    _roleManagerMock.Verify(e => e.Delete(tRole));
    _kerberoIdentityNotifierMock.Verify(e => e.EmitRoleDeleteEvent(new RoleDeleteEvent(tRole.Id)));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Delete_CannotUpdate_ThrowBadRequest()
  {
    var tRoleId = Guid.NewGuid();
    var tRole = new Role { Id = tRoleId };

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tRole);

    _roleManagerMock
      .Setup(e => e.Delete(It.IsAny<Role>()))
      .ReturnsAsync(IdentityResult.Failed());

    var action = () => _roleService.Delete(tRole.Id);

    await action.Should().ThrowAsync<BadRequestException>();

    _roleManagerMock.Verify(e => e.GetById(tRole.Id));
    _roleManagerMock.Verify(e => e.Delete(tRole));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Delete_NotExists_ThrowNotFound()
  {
    var tRoleId = Guid.NewGuid();
    var tRole = new Role { Id = tRoleId };

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var action = () => _roleService.Delete(tRole.Id);

    await action.Should().ThrowAsync<NotFoundException>();

    _roleManagerMock.Verify(e => e.GetById(tRole.Id));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task SetClaims_ValidInput_ReturnCorrectResult()
  {
    var tSetClaimsDto = new RoleSetClaimsDto()
    {
      Claims = new List<ClaimCreateDto>
      {
        new() { Type = "set", Value = "er" },
      },
    };

    var tId = Guid.NewGuid();
    var tRole = new Role();
    var tRoleClaims = new List<Claim>
    {
      new Claim("key", "val")
    };

    var tClaimsToAdd = new List<Claim>();

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tRole);

    _roleManagerMock
      .Setup(e => e.GetClaimsByRole(tRole))
      .ReturnsAsync(tRoleClaims);

    _roleManagerMock
      .Setup(e => e.RemoveClaimFromRole(It.IsAny<Role>(), It.IsAny<Claim>()))
      .ReturnsAsync(IdentityResult.Success);

    _roleManagerMock
      .Setup(e => e.AddClaimToRole(It.IsAny<Role>(), It.IsAny<Claim>()))
      .Callback<Role, Claim>((_, claim) => tClaimsToAdd.Add(claim))
      .ReturnsAsync(IdentityResult.Success);

    await _roleService.SetClaims(tId, tSetClaimsDto);

    _roleManagerMock.Verify(e => e.GetById(tId));
    _roleManagerMock.Verify(e => e.GetClaimsByRole(tRole));
    _roleManagerMock.Verify(e => e.RemoveClaimFromRole(tRole, tRoleClaims.First()));
    _roleManagerMock.Verify(e => e.AddClaimToRole(tRole, tClaimsToAdd.First()));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task SetClaims_NotExists_ThrowNotFound()
  {
    var tSetClaimsDto = new RoleSetClaimsDto()
    {
      Claims = new List<ClaimCreateDto>
      {
        new() { Type = "set", Value = "er" },
      },
    };

    var tId = Guid.NewGuid();

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ThrowsAsync(new InvalidOperationException());

    var action = () => _roleService.SetClaims(tId, tSetClaimsDto);

    await action.Should().ThrowAsync<NotFoundException>();

    _roleManagerMock.Verify(e => e.GetById(tId));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }


  [Fact]
  public async Task SetClaims_CannotRemoveClaims_ThrowBadRequest()
  {
    var tSetClaimsDto = new RoleSetClaimsDto()
    {
      Claims = new List<ClaimCreateDto>
      {
        new() { Type = "set", Value = "er" },
      },
    };

    var tId = Guid.NewGuid();
    var tRole = new Role();
    var tRoleClaims = new List<Claim>
    {
      new Claim("key", "val")
    };

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tRole);

    _roleManagerMock
      .Setup(e => e.GetClaimsByRole(tRole))
      .ReturnsAsync(tRoleClaims);

    _roleManagerMock
      .Setup(e => e.RemoveClaimFromRole(It.IsAny<Role>(), It.IsAny<Claim>()))
      .ReturnsAsync(IdentityResult.Failed());


    var action = () => _roleService.SetClaims(tId, tSetClaimsDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _roleManagerMock.Verify(e => e.GetById(tId));
    _roleManagerMock.Verify(e => e.GetClaimsByRole(tRole));
    _roleManagerMock.Verify(e => e.RemoveClaimFromRole(tRole, tRoleClaims.First()));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task SetClaims_CannotAddClaims_ThrowBadRequest()
  {
    var tSetClaimsDto = new RoleSetClaimsDto()
    {
      Claims = new List<ClaimCreateDto>
      {
        new() { Type = "set", Value = "er" },
      },
    };

    var tId = Guid.NewGuid();
    var tRole = new Role();
    var tRoleClaims = new List<Claim>
    {
      new Claim("key", "val")
    };

    var tClaimsToAdd = new List<Claim>();

    _roleManagerMock
      .Setup(e => e.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tRole);

    _roleManagerMock
      .Setup(e => e.GetClaimsByRole(tRole))
      .ReturnsAsync(tRoleClaims);

    _roleManagerMock
      .Setup(e => e.RemoveClaimFromRole(It.IsAny<Role>(), It.IsAny<Claim>()))
      .ReturnsAsync(IdentityResult.Success);

    _roleManagerMock
      .Setup(e => e.AddClaimToRole(It.IsAny<Role>(), It.IsAny<Claim>()))
      .Callback<Role, Claim>((_, claim) => tClaimsToAdd.Add(claim))
      .ReturnsAsync(IdentityResult.Failed());

    var action = () => _roleService.SetClaims(tId, tSetClaimsDto);

    await action.Should().ThrowAsync<BadRequestException>();

    _roleManagerMock.Verify(e => e.GetById(tId));
    _roleManagerMock.Verify(e => e.GetClaimsByRole(tRole));
    _roleManagerMock.Verify(e => e.RemoveClaimFromRole(tRole, tRoleClaims.First()));
    _roleManagerMock.Verify(e => e.AddClaimToRole(tRole, tClaimsToAdd.First()));

    _roleManagerMock.VerifyNoOtherCalls();
    _userManagerMock.VerifyNoOtherCalls();
    _kerberoIdentityNotifierMock.VerifyNoOtherCalls();
  }
}