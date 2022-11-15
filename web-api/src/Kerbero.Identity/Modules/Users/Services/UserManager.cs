using System.Security.Claims;
using Kerbero.Identity.Common.Models;
using Kerbero.Identity.Modules.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Identity.Modules.Users.Services;

public class UserManager : IUserManager
{
  private readonly UserManager<User> _userManager;

  public UserManager(UserManager<User> userManager)
  {
    _userManager = userManager;
  }

  public Task<List<User>> GetAll()
  {
    return _userManager.Users.ToListAsync();
  }

  public async Task<PaginatedModel<User>> GetPaginated(PaginateModel paginate)
  {
    var queryable = _userManager.Users.AsNoTracking();

    var entities = await queryable
      .Take(paginate.Take)
      .Skip(paginate.Skip)
      .ToListAsync();

    var totalItems = await queryable
      .CountAsync();

    return new PaginatedModel<User>(entities, totalItems);
  }


  /// <inheritdoc />
  public Task<User> GetById(Guid id)
  {
    return _userManager.Users.SingleAsync(e => e.Id == id);
  }

  public async Task<string> GetUserIdAsync(User user)
  {
    return await _userManager.GetUserIdAsync(user);
  }

  public async Task<User?> FindByEmail(string email)
  {
    var user = await _userManager.FindByEmailAsync(email);
    return user; // required because method is from pre Nullable era 
  }

  public Task<User?> FindByRefreshToken(string refreshToken)
  {
    return _userManager.Users.SingleOrDefaultAsync(e => e.RefreshToken == refreshToken);
  }

  public Task<IdentityResult> Create(User user, string password)
  {
    return _userManager.CreateAsync(user, password);
  }

  public async Task<IdentityResult> ConfirmEmailAsync(User user, string code)
  {
    return await _userManager.ConfirmEmailAsync(user, code);
  }

  public Task<IdentityResult> Update(User user)
  {
    return _userManager.UpdateAsync(user);
  }

  public Task<IdentityResult> Delete(User user)
  {
    return _userManager.DeleteAsync(user);
  }

  public Task<string> GenerateEmailConfirmationTokenAsync(User user)
  {
    return _userManager.GenerateEmailConfirmationTokenAsync(user);
  }

  public async Task<List<string>> GetRolesNamesByUser(User user)
  {
    var rolesNames = await _userManager.GetRolesAsync(user);
    return rolesNames.ToList();
  }

  public Task<IdentityResult> AddRoleByNameToUser(User user, string roleName)
  {
    return _userManager.AddToRoleAsync(user, roleName);
  }

  public Task<IdentityResult> AddRolesByNamesToUser(User user, IEnumerable<string> rolesNames)
  {
    return _userManager.AddToRolesAsync(user, rolesNames);
  }

  public Task<IdentityResult> RemoveRolesByNamesFromUser(User user, IEnumerable<string> rolesNames)
  {
    return _userManager.RemoveFromRolesAsync(user, rolesNames);
  }

  public async Task<List<Claim>> GetClaimsByUser(User user)
  {
    var claims = await _userManager.GetClaimsAsync(user);
    return claims.ToList(); // convert to list (we may use a cast for performance in future)
  }

  public Task<IdentityResult> AddClaimsToUser(User user, IEnumerable<Claim> claims)
  {
    return _userManager.AddClaimsAsync(user, claims);
  }

  public Task<IdentityResult> RemoveClaimsToUser(User user, IEnumerable<Claim> claims)
  {
    return _userManager.RemoveClaimsAsync(user, claims);
  }
}