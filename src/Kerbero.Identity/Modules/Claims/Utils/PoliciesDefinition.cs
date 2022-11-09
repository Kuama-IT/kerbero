namespace Kerbero.Identity.Modules.Claims.Utils;

public static class PoliciesDefinition
{
  // ReSharper disable InconsistentNaming

  #region Users
  
  public const string Users_GetAll = "Users.GetAll";
  public const string Users_GetById = "Users.GetById";
  public const string Users_GetRolesById = "Users.GetRolesById";
  public const string Users_GetClaimsById = "Users.GetClaimsById";
  public const string Users_Create = "Users.Create";
  public const string Users_Update = "Users.Update";
  public const string Users_Delete = "Users.Delete";
  public const string Users_SetRoles = "Users.SetRoles";
  public const string Users_SetClaims = "Users.SetClaims";

  #endregion

  #region Roles

  public const string Roles_GetAll = "Roles.GetAll";
  public const string Roles_GetById = "Roles.GetById";
  public const string Roles_GetClaimsById = "Roles.GetClaimsById";
  public const string Roles_Create = "Roles.Create";
  public const string Roles_Update = "Roles.Update";
  public const string Roles_Delete = "Roles.Delete";
  public const string Roles_SetClaims = "Roles.SetClaims";

  #endregion
  
  public const string Claims_GetAll = "Claims.GetAll";
  
  // ReSharper restore InconsistentNaming
}