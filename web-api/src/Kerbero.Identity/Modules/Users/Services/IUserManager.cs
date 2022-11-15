using System.Security.Claims;
using Kerbero.Identity.Common.Models;
using Kerbero.Identity.Modules.Users.Entities;
using Microsoft.AspNetCore.Identity;

namespace Kerbero.Identity.Modules.Users.Services;

public interface IUserManager
{
  Task<List<User>> GetAll();
  Task<PaginatedModel<User>> GetPaginated(PaginateModel paginate);

  /// <exception cref="InvalidOperationException">
  /// No element satisfies the condition,
  /// or more than one element satisfies the condition or
  /// the source contains no elements.
  /// </exception>
  Task<User> GetById(Guid id);

  Task<User?> FindByEmail(string email);

  Task<User?> FindByRefreshToken(string refreshToken);

  /// <summary>
  /// 
  /// </summary>
  /// <param name="user"></param>
  /// <param name="password">will be hashed</param>
  /// <returns></returns>
  Task<IdentityResult> Create(User user, string password);
  Task<IdentityResult> ConfirmEmailAsync(User user, string code);

  Task<IdentityResult> Update(User user);
  Task<IdentityResult> Delete(User user);

  #region Roles

  Task<List<string>> GetRolesNamesByUser(User user);
  Task<IdentityResult> AddRoleByNameToUser(User user, string roleName);

  Task<IdentityResult> AddRolesByNamesToUser(User user, IEnumerable<string> rolesNames);
  Task<IdentityResult> RemoveRolesByNamesFromUser(User user, IEnumerable<string> rolesNames);

  #endregion

  #region Claims

  Task<List<Claim>> GetClaimsByUser(User user);
  Task<IdentityResult> AddClaimsToUser(User user, IEnumerable<Claim> claims);
  Task<IdentityResult> RemoveClaimsToUser(User user, IEnumerable<Claim> claims);

  #endregion

}