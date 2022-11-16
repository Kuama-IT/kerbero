using Kerbero.Identity.Library.Modules.Authentication.Dtos;
using Kerbero.Identity.Library.Modules.Users.Dtos;

namespace Kerbero.Identity.Modules.Authentication.Services;

public interface IAuthenticationService
{
  Task<UserReadDto> Login(LoginDto loginDto);

  public Task Logout();

}
