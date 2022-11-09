using Kerbero.Identity.Library.Modules.Claims.Dtos;

namespace Kerbero.Identity.Library.Modules.Roles.Dtos;

public class RoleSetClaimsDto
{
  public List<ClaimCreateDto> Claims { get; set; } = null!;
}