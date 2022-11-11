using Kerbero.Identity.Modules.Users.Entities;

namespace Kerbero.Identity.Modules.Authentication.Services;

public interface IAuthenticationHelper
{
  Task<string> GenerateAccessToken(User user);
  
  Task<string> GenerateAndSaveRefreshToken(User user);
}