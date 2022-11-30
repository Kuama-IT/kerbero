namespace Kerbero.Identity.Common;

public class KerberoIdentityConfiguration
{
  public TimeSpan CookieExpirationInTimeSpan { get; init; } = TimeSpan.Zero;
  public string SendGridKey { get; set; } = null!;

  // TODO remove
  public string AccessTokenSingKey { get; set; } = null!;
  public double AccessTokenExpirationInMinutes { get; set; }
  public double RefreshTokenExpirationInMinutes { get; set; }
}