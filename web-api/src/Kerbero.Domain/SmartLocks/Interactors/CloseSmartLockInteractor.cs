using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.SmartLocks.Interfaces;
using Kerbero.Domain.SmartLocks.Repositories;

namespace Kerbero.Domain.SmartLocks.Interactors;

public class CloseSmartLockInteractor : ICloseSmartLockInteractor
{
  private readonly INukiSmartLockRepository _nukiSmartLockRepository;
  private readonly IGetNukiCredentialInteractor _getNukiCredentialInteractor;

  public CloseSmartLockInteractor(
    INukiSmartLockRepository nukiSmartLockRepository, IGetNukiCredentialInteractor getNukiCredentialInteractor)
  {
    _nukiSmartLockRepository = nukiSmartLockRepository;
    _getNukiCredentialInteractor = getNukiCredentialInteractor;
  }

  public async Task<Result> Handle(Guid userId, SmartLockProvider smartLockProvider, string smartLockId,
    int credentialId)
  {
    if (smartLockProvider != SmartLockProvider.Nuki)
    {
      return Result.Fail(new UnsupportedSmartLockProviderError());
    }

    // ensure provided credentials belong to current user
    var credentialResult = await _getNukiCredentialInteractor.Handle(credentialId, userId);

    if (credentialResult.IsFailed)
    {
      return credentialResult.ToResult();
    }

    var result =
      await _nukiSmartLockRepository.Close(credentialResult.Value, smartLockId);

    return result;
  }
}