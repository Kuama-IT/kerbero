using Kerbero.Identity.Modules.Users.Entities;
using Microsoft.AspNetCore.Identity;

namespace Kerbero.Identity.Modules.Authentication.Services;

public interface IAuthenticationManager
{
  Task<SignInResult> SignInWithPassword(User user, string password);

  public Task SignOut();
}