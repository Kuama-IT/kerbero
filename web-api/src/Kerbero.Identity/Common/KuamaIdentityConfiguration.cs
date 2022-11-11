namespace Kerbero.Identity.Common;

public class KerberoIdentityConfiguration
{
  public string AccessTokenSingKey { get; set; } = "may the power be with you";
  public int AccessTokenExpirationInMinutes { get; set; } = 5;
  public int RefreshTokenExpirationInMinutes { get; set; } = 48 * 60;
  public string SendGridKey { get; set; } = null!;
}