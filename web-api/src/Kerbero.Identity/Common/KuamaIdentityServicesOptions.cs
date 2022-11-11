using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Kerbero.Identity.Common;

public class KerberoIdentityServicesOptions
{
  public List<Claim>? Claims { get; init; }
  public Action<AuthorizationOptions>? AuthorizationOptionsConfigure { get; init; }
  public bool IsCookieService { get; set; } = false;
}