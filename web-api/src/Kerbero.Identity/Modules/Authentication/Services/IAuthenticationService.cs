using Kerbero.Identity.Library.Modules.Authentication.Dtos;
using Kerbero.Identity.Modules.Authentication.Models;

namespace Kerbero.Identity.Modules.Authentication.Services;

public interface IAuthenticationService
{
  Task<AuthenticatedModel> Login(LoginDto loginDto);
  
}