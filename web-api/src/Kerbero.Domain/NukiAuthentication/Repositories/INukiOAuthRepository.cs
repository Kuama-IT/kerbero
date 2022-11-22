using FluentResults;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiOAuthRepository
{
  public Result<Uri> GetOAuthRedirectUri(string clientId);

  public Task<Result<NukiCredential>> Authenticate(string clientId, string oAuthCode);

  public Task<Result<NukiCredential>> RefreshNukiOAuth(NukiOAuthRequest oAuthRequest);
}