using System.Security.Claims;
using Kerbero.Identity.Common.Models;
using Kerbero.Identity.Modules.Roles.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Identity.Modules.Roles.Services;

public class RoleManager : IRoleManager
{
  private readonly RoleManager<Role> _roleManager;

  public RoleManager(RoleManager<Role> roleManager)
  {
    _roleManager = roleManager;
  }

  public Task<List<Role>> GetAll()
  {
    return _roleManager.Roles.ToListAsync();
  }

  public async Task<PaginatedModel<Role>> GetPaginated(PaginateModel paginate)
  {
    var queryable = _roleManager.Roles.AsNoTracking();

    var entities = await queryable
      .Take(paginate.Take)
      .Skip(paginate.Skip)
      .ToListAsync();

    var totalItems = await queryable
      .CountAsync();

    return new PaginatedModel<Role>(entities, totalItems);
  }

  /// <param name="id"></param>
  /// <inheritdoc />
  public Task<Role> GetById(Guid id)
  {
    return _roleManager.Roles.SingleAsync(e => e.Id == id);
  }

  public Task<List<Role>> GetByNames(IEnumerable<string> names)
  {
    return _roleManager.Roles.Where(e => names.Contains(e.Name)).ToListAsync();
  }

  public async Task<Role?> FindByNameAsync(string name)
  {
    var role = await _roleManager.FindByNameAsync(name);
    return role; // required because method is from pre Nullable era 
  }

  public Task<IdentityResult> Create(Role role)
  {
    return _roleManager.CreateAsync(role);
  }

  public Task<IdentityResult> Update(Role entity)
  {
    return _roleManager.UpdateAsync(entity);
  }

  public Task<IdentityResult> Delete(Role entity)
  {
    return _roleManager.DeleteAsync(entity);
  }

  public async Task<List<Claim>> GetClaimsByRole(Role role)
  {
    var claims = await _roleManager.GetClaimsAsync(role);
    return claims.ToList();
  }

  public Task<IdentityResult> AddClaimToRole(Role role, Claim claim)
  {
    return _roleManager.AddClaimAsync(role, claim);
  }

  public Task<IdentityResult> RemoveClaimFromRole(Role role, Claim claim)
  {
    return _roleManager.RemoveClaimAsync(role, claim);
  }
}