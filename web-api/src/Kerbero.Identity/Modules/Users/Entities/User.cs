using Microsoft.AspNetCore.Identity;

namespace Kerbero.Identity.Modules.Users.Entities;

public class User : IdentityUser<Guid>
{
  public string? RefreshToken { get; set; }
  public DateTime? RefreshTokenExpireDate { get; set; }
}