using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.NukiAuthentication.Models;
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

  public async Task<Result<List<SmartLock>>> GetAll(NukiCredential nukiCredential)
  {
    var result = await _nukiSafeHttpCallHelper.Handle(() =>
      _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("smartlock")
        .WithOAuthBearerToken(nukiCredential.Token)
        .GetJsonAsync<List<NukiSmartLockResponse>>()
    );

    if (result.IsFailed)
    {
      return result.ToResult();
    }

    return SmartLockMapper.Map(result.Value);
  }
}