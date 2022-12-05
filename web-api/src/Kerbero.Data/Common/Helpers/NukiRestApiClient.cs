using System.Net;
using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models.ExternalResponses;
using Kerbero.Domain.SmartLocks.Errors;
using Kerbero.Data.NukiCredentials.Dtos;
using Kerbero.Data.SmartLocks.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kerbero.Data.Common.Helpers;

public class NukiRestApiClient
{
  private readonly ILogger<NukiRestApiClient> _logger;
  private readonly IConfiguration _configuration;

  public NukiRestApiClient(ILogger<NukiRestApiClient> logger, IConfiguration configuration)
  {
    _logger = logger;
    _configuration = configuration;
  }

  public async Task<Result<NukiOAuthResponseDto>> AuthenticateWithCode(string oAuthCode, string redirectUri)
  {
    return await _Handle(
      async () => await _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("oauth")
        .AppendPathSegment("token")
        .PostUrlEncodedAsync(new
        {
          client_id = _configuration["NUKI_CLIENT_ID"],
          client_secret = _configuration["NUKI_CLIENT_SECRET"],
          grant_type = "authorization_code",
          code = oAuthCode,
          redirect_uri = redirectUri
        })
        .ReceiveJson<NukiOAuthResponseDto>());
  }

  public async Task<Result<NukiAccountResponseDto>> GetAccount(string apiToken)
  {
    var account = await _Handle(() =>
      _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("account")
        .WithOAuthBearerToken(apiToken)
        .GetJsonAsync<NukiAccountResponseDto>()
    );

    return account;
  }

  public async Task<Result<IFlurlResponse>> CheckTokenValidity(string apiToken)
  {
    return await _Handle(() =>
      _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("account")
        .WithOAuthBearerToken(apiToken)
        .GetAsync()
    );
  }

  public async Task<Result<List<NukiSmartLockResponse>>> GetAllSmartLocks(string apiToken)
  {
    return await _Handle(() =>
      _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("smartlock")
        .WithOAuthBearerToken(apiToken)
        .GetJsonAsync<List<NukiSmartLockResponse>>()
    );
  }

  public async Task<Result<NukiSmartLockResponse>> GetSmartLock(string id, string apiToken)
  {
    return await _Handle(() =>
      _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("smartlock")
        .AppendPathSegment(id)
        .WithOAuthBearerToken(apiToken)
        .GetJsonAsync<NukiSmartLockResponse>()
    );
  }

  public async Task<Result<IFlurlResponse>> OpenSmartLock(string id, string apiToken)
  {
    return await _Handle(() =>
      _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("smartlock")
        .AppendPathSegment(id)
        .AppendPathSegment("action")
        .AppendPathSegment("unlock")
        .WithOAuthBearerToken(apiToken)
        .PostAsync()
    );
  }

  public async Task<Result<IFlurlResponse>> CloseSmartLock(string id, string apiToken)
  {
    return await _Handle(() =>
      _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("smartlock")
        .AppendPathSegment(id)
        .AppendPathSegment("action")
        .AppendPathSegment("lock")
        .WithOAuthBearerToken(apiToken)
        .PostAsync()
    );
  }

  private async Task<Result<TResponse>> _Handle<TResponse>(Func<Task<TResponse>> call)
  {
    try
    {
      var response = await call.Invoke();

      if (response is null)
      {
        return Result.Fail(new UnableToParseResponseError("Response is null"));
      }

      return Result.Ok(response);
    }

    #region ErrorManagement

    catch (FlurlHttpException exception)
    {
      _logger.LogError(exception, "Error while calling nuki Apis with request");
      var rawResponse = await exception.GetResponseStringAsync();
      _logger.LogDebug("Raw api response: {RawResponse}", rawResponse);
      if (exception.StatusCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.MethodNotAllowed
          or (int)HttpStatusCode.Forbidden)
      {
        var error = await exception.GetResponseJsonAsync<NukiErrorExternalResponse>();
        if (error?.Error?.Contains("invalid") == true)
        {
          return Result.Fail(new InvalidParametersError(error.Error + ": " + error.ErrorMessage));
        }

        return Result.Fail(new UnauthorizedAccessError());
      }

      if (exception.StatusCode is (int)HttpStatusCode.BadRequest)
      {
        return Result.Fail(new InvalidParametersError(exception.Call.HttpRequestMessage.RequestUri!.PathAndQuery));
      }

      if (exception.StatusCode is (int)HttpStatusCode.NotFound)
      {
        return Result.Fail(new SmartLockNotFoundError(exception.Call.HttpRequestMessage.RequestUri!.Segments.Last()));
      }

      if (exception.StatusCode is (int)HttpStatusCode.RequestTimeout or >= 500)
      {
        return Result.Fail(new ExternalServiceUnreachableError());
      }

      return Result.Fail(new UnknownExternalError());
    }
    catch (ArgumentNullException exception)
    {
      _logger.LogError(exception, "Error while calling nuki Apis with request");
      return Result.Fail(new InvalidParametersError("options"));
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while calling nuki Apis with request");
      return Result.Fail(new KerberoError());
    }

    #endregion
  }
}