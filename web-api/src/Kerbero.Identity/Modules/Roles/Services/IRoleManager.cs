using System.Security.Claims;
using Kerbero.Identity.Common.Models;
using Kerbero.Identity.Modules.Roles.Entities;
using Microsoft.AspNetCore.Identity;

namespace Kerbero.Identity.Modules.Roles.Services;

public interface IRoleManager
{
  Task<List<Role>> GetAll();
  Task<PaginatedModel<Role>> GetPaginated(PaginateModel paginate);

  /// <param name="id"></param>
  /// <exception cref="InvalidOperationException">
  /// No element satisfies the condition,
  /// or more than one element satisfies the condition or
  /// the source contains no elements.
  /// </exception>
  Task<Role> GetById(Guid id);

  Task<List<Role>> GetByNames(IEnumerable<string> names);

  Task<Role?> FindByNameAsync(string name);
  Task<IdentityResult> Create(Role role);
  Task<IdentityResult> Update(Role entity);
  Task<IdentityResult> Delete(Role entity);

  #region Claims

  Task<List<Claim>> GetClaimsByRole(Role role);
  Task<IdentityResult> AddClaimToRole(Role role, Claim claim);
  Task<IdentityResult> RemoveClaimFromRole(Role role, Claim claim);

  #endregion

}