using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Models;
using Kerbero.Domain.SmartLocks.Repositories;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.SmartLocks.Mappers;
using Microsoft.Extensions.Configuration;

namespace Kerbero.Infrastructure.SmartLocks.Repositories;

public class NukiSmartLockRepository : INukiSmartLockRepository
{
  private readonly IConfiguration _configuration;
  private readonly NukiRestApiClient _nukiRestApiClient;

  public NukiSmartLockRepository(IConfiguration configuration, NukiRestApiClient nukiRestApiClient)
  {
    _configuration = configuration;
    _nukiRestApiClient = nukiRestApiClient;
  }

  public async Task<Result<List<SmartLockModel>>> GetAll(NukiCredentialModel nukiCredentialModel)
  {
    var result = await _nukiRestApiClient
      .GetAllSmartLocks(nukiCredentialModel.Token);

    if (result.IsFailed)
    {
      return result.ToResult();
    }

    return SmartLockMapper.Map(result.Value);
  }

  public async Task<Result<SmartLockModel>> Get(NukiCredentialModel nukiCredentialModel, string id)
  {
    var result = await _nukiRestApiClient
      .GetSmartLock(id, nukiCredentialModel.Token);

    if (result.IsFailed)
    {
      return result.ToResult();
    }

    return SmartLockMapper.Map(result.Value);
  }

  public async Task<Result> Open(NukiCredentialModel nukiCredentialModel, string id)
  {
    var result = await _nukiRestApiClient
      .OpenSmartLock(id, nukiCredentialModel.Token);

    if (result.IsFailed)
    {
      return result.ToResult();
    }

    return Result.Ok();
  }

  public async Task<Result> Close(NukiCredentialModel nukiCredentialModel, string id)
  {
    var result = await _nukiRestApiClient
      .CloseSmartLock(id, nukiCredentialModel.Token);

    if (result.IsFailed)
    {
      return result.ToResult();
    }

    return Result.Ok();
  }
}