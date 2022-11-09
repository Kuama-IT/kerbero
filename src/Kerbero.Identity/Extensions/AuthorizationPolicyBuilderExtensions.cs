using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Kerbero.Identity.Extensions;

public static class AuthorizationPolicyBuilderExtensions
{
  public static void RequireClaim(this AuthorizationPolicyBuilder policyBuilder, Claim claim)
  {
    policyBuilder.Requirements.Add(
      new ClaimsAuthorizationRequirement(claim.Type, allowedValues: new[] { claim.Value })
    );
  }
}