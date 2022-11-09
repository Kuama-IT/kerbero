using Kerbero.Identity.Modules.Claims.Utils;
using Microsoft.AspNetCore.Authorization;

namespace Kerbero.Identity.Extensions;

public static class AuthorizationOptionsExtensions
{
  public static void AddKuIdentityPolicies(this AuthorizationOptions options)
  {
    #region Users

    options.AddPolicy(PoliciesDefinition.Users_GetAll, p => p.RequireClaim(ClaimsDefinition.Users_GetAll));
    options.AddPolicy(PoliciesDefinition.Users_GetById, p => p.RequireClaim(ClaimsDefinition.Users_GetById));
    options.AddPolicy(PoliciesDefinition.Users_GetRolesById, p => p.RequireClaim(ClaimsDefinition.Users_GetRolesById));
    options.AddPolicy(PoliciesDefinition.Users_GetClaimsById, p => p.RequireClaim(ClaimsDefinition.Users_GetClaimsById));
    options.AddPolicy(PoliciesDefinition.Users_Create, p => p.RequireClaim(ClaimsDefinition.Users_Create));
    options.AddPolicy(PoliciesDefinition.Users_Update, p => p.RequireClaim(ClaimsDefinition.Users_Update));
    options.AddPolicy(PoliciesDefinition.Users_Delete, p => p.RequireClaim(ClaimsDefinition.Users_Delete));
    options.AddPolicy(PoliciesDefinition.Users_SetRoles, p => p.RequireClaim(ClaimsDefinition.Users_SetRoles));
    options.AddPolicy(PoliciesDefinition.Users_SetClaims, p => p.RequireClaim(ClaimsDefinition.Users_SetClaims));

    #endregion

    #region Roles

    options.AddPolicy(PoliciesDefinition.Roles_GetAll, p => p.RequireClaim(ClaimsDefinition.Roles_GetAll));
    options.AddPolicy(PoliciesDefinition.Roles_GetById, p => p.RequireClaim(ClaimsDefinition.Roles_GetById));
    options.AddPolicy(PoliciesDefinition.Roles_GetClaimsById, p => p.RequireClaim(ClaimsDefinition.Roles_GetClaimsById));
    options.AddPolicy(PoliciesDefinition.Roles_Create, p => p.RequireClaim(ClaimsDefinition.Roles_Create));
    options.AddPolicy(PoliciesDefinition.Roles_Update, p => p.RequireClaim(ClaimsDefinition.Roles_Update));
    options.AddPolicy(PoliciesDefinition.Roles_Delete, p => p.RequireClaim(ClaimsDefinition.Roles_Delete));
    options.AddPolicy(PoliciesDefinition.Roles_SetClaims, p => p.RequireClaim(ClaimsDefinition.Roles_SetClaims));

    #endregion

    options.AddPolicy(PoliciesDefinition.Claims_GetAll, p => p.RequireClaim(ClaimsDefinition.Claims_GetAll));
  }
}