using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Kerbero.Identity.Modules.Claims.Services;
using Xunit;

namespace Kerbero.Identity.Tests.Modules.Claims.Services;

public class ClaimManagerTest
{
  [Fact]
  public void AddRange_ValidInput_ShouldAddClaims()
  {
    var tClaim = new Claim("key1", "val1");
    var tClaims = new List<Claim>
    {
      tClaim
    };

    var service = new ClaimManager();

    var actualBefore = service.GetAll();
    service.AddRange(tClaims);
    var actualAfter = service.GetAll();

    actualAfter.Count.Should().Be(actualBefore.Count + 1);
    actualAfter.Should().Contain(tClaim);
  }
  
  [Fact]
  public void Exists_ValidInput_ReturnCorrectResult()
  {
    var tClaim = new Claim("key1", "val1");
    var tClaims = new List<Claim>
    {
      tClaim
    };

    var service = new ClaimManager();
    
    service.AddRange(tClaims);
    
    var exists = service.Exists(tClaim.Type, tClaim.Value);
    exists.Should().BeTrue();
    
    var doNotExists = service.Exists("maggot", "random");
    doNotExists.Should().BeFalse();
  }
}