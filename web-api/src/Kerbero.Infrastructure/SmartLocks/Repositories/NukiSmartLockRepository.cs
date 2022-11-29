using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Models;
using Kerbero.Domain.SmartLocks.Repositories;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.SmartLocks.Mappers;
using Kerbero.Infrastructure.SmartLocks.Models;
using Microsoft.Extensions.Configuration;

namespace Kerbero.Infrastructure.SmartLocks.Repositories;

public class NukiSmartLockRepository : INukiSmartLockRepository
{
  private readonly IConfiguration _configuration;
  private readonly NukiSafeHttpCallHelper _nukiSafeHttpCallHelper;

  public NukiSmartLockRepository(IConfiguration configuration, NukiSafeHttpCallHelper nukiSafeHttpCallHelper)
  {
    _configuration = configuration;
    _nukiSafeHttpCallHelper = nukiSafeHttpCallHelper;
  }

  public async Task<Result<List<SmartLock>>> GetAll(NukiCredentialModel nukiCredentialModel)
  {
    var result = await _nukiSafeHttpCallHelper.Handle(() =>
      _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("smartlock")
        .WithOAuthBearerToken(nukiCredentialModel.Token)
        .GetJsonAsync<List<NukiSmartLockResponse>>()
    );

    if (result.IsFailed)
    {
      return result.ToResult();
    }

    return SmartLockMapper.Map(result.Value);
  }

  public async Task<Result<SmartLock>> Get(NukiCredentialModel nukiCredentialModel, string id)
  {
    var result = await _nukiSafeHttpCallHelper.Handle(() =>
      _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("smartlock")
        .AppendPathSegment(id)
        .WithOAuthBearerToken(nukiCredentialModel.Token)
        .GetJsonAsync<NukiSmartLockResponse>()
    );

    if (result.IsFailed)
    {
      return result.ToResult();
    }

    return SmartLockMapper.Map(result.Value);
  }

  public async Task<Result> Open(NukiCredentialModel nukiCredentialModel, string id)
  {
    var result = await _nukiSafeHttpCallHelper.Handle(() =>
      _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("smartlock")
        .AppendPathSegment(id)
        .AppendPathSegment("action")
        .AppendPathSegment("unlock")
        .WithOAuthBearerToken(nukiCredentialModel.Token)
        .PostAsync()
    );

    if (result.IsFailed)
    {
      return result.ToResult();
    }

    return Result.Ok();
  }
}