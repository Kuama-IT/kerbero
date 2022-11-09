namespace Kerbero.Identity.Library.Modules.Authentication.Dtos;

public class AuthenticatedDto
{
  public string AccessToken { get; set; } = null!;
  public string RefreshToken { get; set; } = null!;
}