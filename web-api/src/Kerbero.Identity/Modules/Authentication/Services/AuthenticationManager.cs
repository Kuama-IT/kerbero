using Kerbero.Identity.Modules.Users.Entities;
using Microsoft.AspNetCore.Identity;

namespace Kerbero.Identity.Modules.Authentication.Services;

public class AuthenticationManager : IAuthenticationManager
{
  private readonly SignInManager<User> _signInManager;

  public AuthenticationManager(SignInManager<User> signInManager)
  {
    _signInManager = signInManager;
  }

  public Task<SignInResult> SignInWithPassword(User user, string password)
  {
    return _signInManager.PasswordSignInAsync(user, password, false, false);
  }
}