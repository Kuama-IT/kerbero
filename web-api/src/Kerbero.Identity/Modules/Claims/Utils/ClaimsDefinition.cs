using System.Security.Claims;

namespace Kerbero.Identity.Modules.Claims.Utils;

public static class ClaimsDefinition
{
  private const string KuIdentityClaimType = "KU-IDENTITY";

  // ReSharper disable InconsistentNaming

  #region Users

  public static readonly Claim Users_GetAll = new(KuIdentityClaimType, "Users.GetAll");
  public static readonly Claim Users_GetById = new(KuIdentityClaimType, "Users.GetById");
  public static readonly Claim Users_GetRolesById = new(KuIdentityClaimType, "Users.GetRolesById");
  public static readonly Claim Users_GetClaimsById = new(KuIdentityClaimType, "Users.GetClaimsById");
  public static readonly Claim Users_Create = new(KuIdentityClaimType, "Users.Create");
  public static readonly Claim Users_Update = new(KuIdentityClaimType, "Users.Update");
  public static readonly Claim Users_Delete = new(KuIdentityClaimType, "Users.Delete");
  public static readonly Claim Users_SetRoles = new(KuIdentityClaimType, "Users.SetRoles");
  public static readonly Claim Users_SetClaims = new(KuIdentityClaimType, "Users.SetClaims");

  #endregion

  #region Roles

  public static readonly Claim Roles_GetAll = new(KuIdentityClaimType, "Roles.GetAll");
  public static readonly Claim Roles_GetById = new(KuIdentityClaimType, "Roles.GetById");
  public static readonly Claim Roles_GetClaimsById = new(KuIdentityClaimType, "Roles.GetClaimsById");
  public static readonly Claim Roles_Create = new(KuIdentityClaimType, "Roles.Create");
  public static readonly Claim Roles_Update = new(KuIdentityClaimType, "Roles.Update");
  public static readonly Claim Roles_Delete = new(KuIdentityClaimType, "Roles.Delete");
  public static readonly Claim Roles_SetClaims = new(KuIdentityClaimType, "Roles.SetClaims");

  #endregion

  public static readonly Claim Claims_GetAll = new(KuIdentityClaimType, "Claims.GetAll");
  

  
  public static readonly List<Claim> KuIdentityClaims = new()
  {
    #region Users

    Users_GetAll,
    Users_GetById,
    Users_GetRolesById,
    Users_GetClaimsById,
    Users_Create,
    Users_Update,
    Users_Delete,
    Users_SetRoles,
    Users_SetClaims,
    
    #endregion

    #region Roles

    Roles_GetAll,
    Roles_GetById,
    Roles_GetClaimsById,
    Roles_Create,
    Roles_Update,
    Roles_Delete,
    Roles_SetClaims,

    #endregion
    
    Claims_GetAll,
  };
  
  // ReSharper restore InconsistentNaming
}
