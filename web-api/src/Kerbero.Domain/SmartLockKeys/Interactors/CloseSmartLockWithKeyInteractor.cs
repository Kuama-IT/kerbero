using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Domain.SmartLocks.Interfaces;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class CloseSmartLockWithKeyInteractor : ICloseSmartLockWithKeyInteractor
{
  private readonly ISmartLockKeyRepository _smartLockKeyRepository;
  private readonly ICloseSmartLockInteractor _closeSmartLockInteractor;
  private readonly IGetNukiCredentialInteractor _getNukiCredentialInteractor;

  public CloseSmartLockWithKeyInteractor(
    ISmartLockKeyRepository smartLockKeyRepository,
    ICloseSmartLockInteractor closeSmartLockInteractor,
    IGetNukiCredentialInteractor getNukiCredentialInteractor)
  {
    _smartLockKeyRepository = smartLockKeyRepository;
    _closeSmartLockInteractor = closeSmartLockInteractor;
    _getNukiCredentialInteractor = getNukiCredentialInteractor;
  }

  public async Task<Result> Handle(Guid smartLockKeyId, string smartLockKeyPassword)
  {
    var smartLockKeyResult = await _smartLockKeyRepository.GetById(smartLockKeyId);
    if (smartLockKeyResult.IsFailed)
    {
      return Result.Fail(smartLockKeyResult.Errors);
    }

    var smartLockKey = smartLockKeyResult.Value;

    var smartLockProvider = SmartLockProvider.TryParse(smartLockKey.SmartLockProvider);
    if (smartLockProvider != SmartLockProvider.Nuki)
    {
      return Result.Fail(new UnsupportedSmartLockProviderError());
    }
    
    var validationResult = smartLockKey.CanOperateWith(smartLockKeyPassword);
    if (validationResult.IsFailed)
    {
      return Result.Fail(validationResult.Errors);
    }

    var nukiCredentialResult = await _getNukiCredentialInteractor.Handle(smartLockKey.CredentialId);
    if (nukiCredentialResult.IsFailed)
    {
      return Result.Fail(nukiCredentialResult.Errors);
    }

    var closeResult = await _closeSmartLockInteractor.Handle(
      nukiCredentialResult.Value.UserId,
      smartLockProvider,
      smartLockKeyResult.Value.SmartLockId,
      nukiCredentialResult.Value.Id
    );

    if (closeResult.IsFailed)
    {
      return Result.Fail(closeResult.Errors);
    }

    // update usage on smartlock key
    smartLockKey.UsageCounter++;
    var updateSmartLockKeyResult = await _smartLockKeyRepository.Update(smartLockKey);
    if (updateSmartLockKeyResult.IsFailed)
    {
      return Result.Fail(updateSmartLockKeyResult.Errors);
    }

    return Result.Ok();
  }
}