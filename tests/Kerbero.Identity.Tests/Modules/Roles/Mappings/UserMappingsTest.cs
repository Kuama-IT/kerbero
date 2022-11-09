using System;
using System.Collections.Generic;
using FluentAssertions;
using Kerbero.Identity.Library.Modules.Roles.Dtos;
using Kerbero.Identity.Modules.Roles.Entities;
using Kerbero.Identity.Modules.Roles.Mappings;
using Xunit;

namespace Kerbero.Identity.Tests.Modules.Roles.Mappings;

public class RoleMappingsTest
{
  [Fact]
  public void Map_From_ListEntities_To_ListReadDto()
  {
    var tEntities = new List<Role>
    {
      new()
      {
        Id = Guid.Empty,
        Name = "nam",
        ConcurrencyStamp = "stamp",
      }
    };

    var actual = RoleMappings.Map(tEntities);

    var expected = new List<RoleReadDto>
    {
      new()
      {
        Id = Guid.Empty,
        Name = "nam",
      }
    };

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Map_From_Entity_To_ReadDto()
  {
    var tEntity = new Role
    {
      Id = Guid.Empty,
      Name = "nam",
      ConcurrencyStamp = "stamp",
    };

    var actual = RoleMappings.Map(tEntity);

    var expected = new RoleReadDto()
    {
      Id = Guid.Empty,
      Name = "nam",
    };

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Map_From_CreateDto_To_Entity()
  {
    var tCreateDto = new RoleCreateDto
    {
      Name = "nam",
    };

    var actual = RoleMappings.Map(tCreateDto);

    var expected = new RoleReadDto()
    {
      Id = Guid.Empty,
      Name = "nam",
    };

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Patch_UpdateDto_To_Entity()
  {
    var tUpdateDto = new RoleUpdateDto
    {
      Name = "nam",
    };

    var tEntity = new Role
    {
      Name = "nam",
      ConcurrencyStamp = "stamp"
    };

    RoleMappings.Patch(tUpdateDto, tEntity);


    tEntity.Name.Should().Be(tUpdateDto.Name);
    tEntity.ConcurrencyStamp.Should().Be("stamp");
  }
}