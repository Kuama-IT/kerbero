using System;
using System.Collections.Generic;
using FluentAssertions;
using Kerbero.Identity.Library.Modules.Users.Dtos;
using Kerbero.Identity.Modules.Users.Entities;
using Kerbero.Identity.Modules.Users.Mappings;
using Xunit;

namespace Kerbero.Identity.Tests.Modules.Users.Mappings;

public class UserMappingsTest
{
  [Fact]
  public void Map_From_ListEntities_To_ListReadDto()
  {
    var tEntities = new List<User>
    {
      new()
      {
        Id = Guid.Empty,
        Email = "email",
        EmailConfirmed = true,
        PasswordHash = "hash",
        ConcurrencyStamp = "stamp",
        UserName = "username",
      }
    };

    var actual = UserMappings.Map(tEntities);

    var expected = new List<UserReadDto>
    {
      new()
      {
        Id = Guid.Empty,
        Email = "email",
        EmailConfirmed = true,
        UserName = "username",
      }
    };

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Map_From_Entity_To_ReadDto()
  {
    var tEntity = new User
    {
      Id = Guid.Empty,
      Email = "email",
      EmailConfirmed = true,
      PasswordHash = "hash",
      ConcurrencyStamp = "stamp",
      UserName = "username",
    };

    var actual = UserMappings.Map(tEntity);

    var expected = new UserReadDto()
    {
      Id = Guid.Empty,
      Email = "email",
      EmailConfirmed = true,
      UserName = "username",
    };

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Map_From_CreateDto_To_Entity()
  {
    var tCreateDto = new UserCreateDto
    {
      Email = "email",
      UserName = "username",
      Password = "pass"
    };

    var actual = UserMappings.Map(tCreateDto);

    var expected = new UserReadDto()
    {
      Id = Guid.Empty,
      Email = "email",
      EmailConfirmed = false,
      UserName = "username",
    };

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Patch_UpdateDto_To_Entity()
  {
    var tUpdateDto = new UserUpdateDto
    {
      Email = "email",
      UserName = "username",
    };

    var tEntity = new User
    {
      Email = "oldemail",
      UserName = "oldusername",
      EmailConfirmed = false,
    };

    UserMappings.Patch(tUpdateDto, tEntity);


    tEntity.UserName.Should().Be(tUpdateDto.UserName);
    tEntity.Email.Should().Be(tUpdateDto.Email);
    tEntity.EmailConfirmed.Should().Be(false);
  }
}