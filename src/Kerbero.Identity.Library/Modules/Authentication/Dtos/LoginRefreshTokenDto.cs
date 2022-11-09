using System.ComponentModel.DataAnnotations;

namespace Kerbero.Identity.Library.Modules.Authentication.Dtos;

public class LoginRefreshTokenDto
{
  [Required]
  public string Token { get; set; } = null!;
}