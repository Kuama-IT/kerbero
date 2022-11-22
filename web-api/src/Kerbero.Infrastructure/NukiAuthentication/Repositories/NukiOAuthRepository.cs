using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.NukiAuthentication.Mappers;
using Kerbero.Infrastructure.NukiAuthentication.Models;
using Microsoft.Extensions.Configuration;
using ArgumentNullException = System.ArgumentNullException;

namespace Kerbero.Infrastructure.NukiAuthentication.Repositories;

public class NukiOAuthRepository : INukiOAuthRepository
{
  private readonly IConfiguration _configuration;
  private readonly NukiSafeHttpCallHelper _nukiSafeHttpCallHelper;

  public NukiOAuthRepository(IConfiguration configuration,
    NukiSafeHttpCallHelper nukiSafeHttpCallHelper)
  {
    _configuration = configuration;
    _nukiSafeHttpCallHelper = nukiSafeHttpCallHelper;
  }

  /// <summary>
  /// Builds a Uri where the user who wants to authenticate should be redirected
  /// </summary>
  /// <param name="clientId"></param>
  /// <returns />
  public Result<Uri> GetOAuthRedirectUri(string clientId)
  {
    if (string.IsNullOrEmpty(clientId))
    {
      return Result.Fail(new InvalidParametersError("client_id"));
    }

    try
    {
      var redirectUriClientId = _configuration["ALIAS_DOMAIN"]
        .AppendPathSegment(_configuration["NUKI_REDIRECT_FOR_TOKEN"])
        .AppendPathSegment(clientId);

      var uri = _configuration["NUKI_DOMAIN"]
        .AppendPathSegments("oauth", "authorize")
        .SetQueryParams(new
        {
          response_type = "code",
          client_id = clientId,
          redirect_uri = redirectUriClientId.ToString(),
          scope = _configuration["NUKI_SCOPES"]
        })
        .ToUri();

      return uri;
    }
    catch (ArgumentNullException)
    {
      return Result.Fail(new InvalidParametersError("options"));
    }
    catch (Exception)
    {
      return Result.Fail(new KerberoError());
    }
  }

  public async Task<Result<NukiCredential>> Authenticate(string clientId, string oAuthCode)
  {
    if (string.IsNullOrWhiteSpace(clientId))
    {
      return Result.Fail(new InvalidParametersError("client_id"));
    }

    Url redirectUriClientId;
    try
    {
      redirectUriClientId = _configuration["ALIAS_DOMAIN"]
        .AppendPathSegment(_configuration["NUKI_REDIRECT_FOR_TOKEN"])
        .AppendPathSegment(clientId);
    }
    catch (ArgumentNullException)
    {
      return Result.Fail(new InvalidParametersError("options"));
    }

    var result = await _ExecuteApiRequest(clientId, new
    {
      client_id = clientId,
      client_secret = _configuration["NUKI_CLIENT_SECRET"],
      grant_type = "authorization_code",
      code = oAuthCode,
      redirect_uri = redirectUriClientId.ToString()
    });

    return result;
  }

  /// <summary>
  ///  Update the authentication token with refresh token
  /// </summary>
  /// <param name="oAuthRequest"></param>
  /// <returns></returns>
  public async Task<Result<NukiCredential>> RefreshNukiOAuth(NukiOAuthRequest oAuthRequest)
  {
    if (string.IsNullOrWhiteSpace(oAuthRequest.ClientId))
    {
      return Result.Fail(new InvalidParametersError("client_id"));
    }

    return await _ExecuteApiRequest(oAuthRequest.ClientId, new
    {
      client_id = oAuthRequest.ClientId,
      client_secret = _configuration["NUKI_CLIENT_SECRET"],
      grant_type = "refresh_token",
      refresh_token = oAuthRequest.RefreshToken,
      scope = _configuration["NUKI_SCOPES"]
    });
  }

  // TODO would be nice to type the body
  private async Task<Result<NukiCredential>> _ExecuteApiRequest(string clientId, object body)
  {
    var response = await _nukiSafeHttpCallHelper.Handle(
      async () => await _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("oauth")
        .AppendPathSegment("token")
        .PostUrlEncodedAsync(body)
        .ReceiveJson<NukiOAuthResponse>());


    if (response.IsFailed)
    {
      return response.ToResult();
    }

    var createdAt = DateTime.UtcNow;
    var model = NukiCredentialMapper.Map(response.Value, clientId, createdAt);
    return model;
  }
}