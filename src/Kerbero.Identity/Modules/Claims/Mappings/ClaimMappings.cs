using System.Security.Claims;
using Kerbero.Identity.Library.Modules.Claims.Dtos;

namespace Kerbero.Identity.Modules.Claims.Mappings;

public static class ClaimMappings
{
  public static List<ClaimReadDto> Map(IEnumerable<Claim> claims)
  {
    return claims.Select(Map).ToList();
  }

  public static ClaimReadDto Map(Claim claim)
  {
    return new ClaimReadDto
    {
      Type = claim.Type,
      Value = claim.Value,
    };
  }

  public static List<Claim> Map(List<ClaimCreateDto> createDtos)
  {
    return createDtos.ConvertAll(Map);
  }

  public static Claim Map(ClaimCreateDto createCreateDto)
  {
    return new Claim(createCreateDto.Type, createCreateDto.Value);
  }
}