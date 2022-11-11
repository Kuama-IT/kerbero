using System.ComponentModel.DataAnnotations;

namespace Kerbero.Identity.Library.Modules.Authentication.Dtos;

public class LoginDto
{
  [Required]
  [EmailAddress]
  public string Email { get; set; } = null!;
  
  [Required]
  public string Password { get; set; } = null!;
}