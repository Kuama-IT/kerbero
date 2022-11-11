using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Kerbero.Identity.Library.Modules.Claims.Dtos;
using Kerbero.Identity.Modules.Claims.Mappings;
using Xunit;

namespace Kerbero.Identity.Tests.Modules.Claims.Mappings;

public class ClaimMappingsTest
{
  [Fact]
  public void Map_From_ListClaim_To_ListClaimReadDto()
  {
    var tClaims = new List<Claim>
    {
      new("type1", "value1")
    };

    var actual = ClaimMappings.Map(tClaims);

    var expected = new List<ClaimReadDto>
    {
      new()
      {
        Type = "type1",
        Value = "value1",
      }
    };

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Map_From_Claim_To_ClaimReadDto()
  {
    var tClaim = new Claim("type1", "value1");

    var actual = ClaimMappings.Map(tClaim);

    var expected = new ClaimReadDto
    {
      Type = "type1",
      Value = "value1",
    };

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Map_From_ListCreateDto_To_ListClaim()
  {
    var tClaims = new List<ClaimCreateDto>
    {
      new()
      {
        Type = "type1",
        Value = "value1",
      },
    };

    var actual = ClaimMappings.Map(tClaims);

    var expected = new List<Claim>
    {
      new("type1", "value1")
    };

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Map_From_CreateDto_To_Claim()
  {
    var tClaim = new ClaimCreateDto
    {
      Type = "type1",
      Value = "value1",
    };

    var actual = ClaimMappings.Map(tClaim);

    var expected = new Claim("type1", "value1");

    actual.Should().BeEquivalentTo(expected);
  }
}