using Kerbero.Identity.Library.Modules.Authentication.Dtos;

namespace Kerbero.Identity.Modules.Authentication.Services;

public interface IAuthenticationService
{
  Task<AuthenticatedDto> LoginWithEmail(LoginEmailDto loginEmailDto);
  
  Task<AuthenticatedDto> LoginWithRefreshToken(LoginRefreshTokenDto loginRefreshTokenDto);
}