using Kerbero.Identity.Common.Exceptions;
using Kerbero.Identity.Modules.Users.Services;
using Kerbero.Identity.Library.Modules.Authentication.Dtos;
using Kerbero.Identity.Library.Modules.Users.Dtos;
using Kerbero.Identity.Modules.Users.Mappings;

namespace Kerbero.Identity.Modules.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
  private readonly IUserManager _userManager;
  private readonly IAuthenticationManager _authenticationManager;

  public AuthenticationService(
    IUserManager userManager,
    IAuthenticationManager authenticationManager
    )
  {
    _userManager = userManager;
    _authenticationManager = authenticationManager;
  }

  public async Task<UserReadDto> Login(LoginDto loginDto)
  {
    var user = await _userManager.FindByEmail(loginDto.Email);
    if (user is null)
    {
      throw new UnauthorizedException();
    }
    
    var signInResult = await _authenticationManager.SignInWithPassword(user, loginDto.Password);
    if (!signInResult.Succeeded)
    {
      throw new UnauthorizedException();
    }

    return UserMappings.Map(user);
  }
  
  public async Task Logout()
  {
    await _authenticationManager.SignOut();
  }
  
}
