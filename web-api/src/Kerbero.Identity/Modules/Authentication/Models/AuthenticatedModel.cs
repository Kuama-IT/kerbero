using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Kerbero.Identity.Modules.Authentication.Models;

public class AuthenticatedModel
{
  public AuthenticationProperties Properties { get; init; } = null!;
  public ClaimsIdentity ClaimIdentity { get; init; } = null!;
}