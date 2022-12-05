using FluentResults;
using Kerbero.Domain.NukiCredentials.Errors;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Models;
using Kerbero.Domain.SmartLocks.Repositories;
using Kerbero.Data.Common.Helpers;
using Kerbero.Data.SmartLocks.Mappers;

namespace Kerbero.Data.SmartLocks.Repositories;

public class NukiSmartLockRepository : INukiSmartLockRepository
{
  private readonly NukiRestApiClient _nukiRestApiClient;

  public NukiSmartLockRepository(NukiRestApiClient nukiRestApiClient)
  {
    _nukiRestApiClient = nukiRestApiClient;
  }

  public async Task<Result<List<SmartLockModel>>> GetAll(NukiCredentialModel nukiCredentialModel)
  {
    if (string.IsNullOrEmpty(nukiCredentialModel.Token))
    {
      return Result.Fail(new NukiCredentialInvalidTokenError());
    }
    
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
    if (string.IsNullOrEmpty(nukiCredentialModel.Token))
    {
      return Result.Fail(new NukiCredentialInvalidTokenError());
    }

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
    if (string.IsNullOrEmpty(nukiCredentialModel.Token))
    {
      return Result.Fail(new NukiCredentialInvalidTokenError());
    }

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
    if (string.IsNullOrEmpty(nukiCredentialModel.Token))
    {
      return Result.Fail(new NukiCredentialInvalidTokenError());
    }

    var result = await _nukiRestApiClient
      .CloseSmartLock(id, nukiCredentialModel.Token);

    if (result.IsFailed)
    {
      return result.ToResult();
    }

    return Result.Ok();
  }
}