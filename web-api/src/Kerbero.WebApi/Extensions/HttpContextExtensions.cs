using System.Security.Claims;

namespace Kerbero.WebApi.Extensions;

public static class HttpContextExtensions
{
  public static Guid GetAuthenticatedUserId(this HttpContext httpContext)
  {
    var nameIdentifierClaim = httpContext.User.Claims.Single(claim => claim.Type == ClaimTypes.NameIdentifier);
    return Guid.Parse(nameIdentifierClaim.Value);
  }
}