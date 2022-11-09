using Kerbero.Identity.Library.Modules.Claims.Dtos;

namespace Kerbero.Identity.Library.Modules.Users.Dtos;

public class UserSetClaimsDto
{
  public List<ClaimCreateDto> Claims { get; set; } = null!;
}