using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Domain.SmartLocks.Interfaces;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class OpenSmartLockWithKeyInteractor : IOpenSmartLockWithKeyInteractor
{
  private readonly ISmartLockKeyRepository _smartLockKeyRepository;
  private readonly IOpenSmartLockInteractor _openSmartLockInteractor;
  private readonly IGetNukiCredentialInteractor _getNukiCredentialInteractor;

  public OpenSmartLockWithKeyInteractor(
    ISmartLockKeyRepository smartLockKeyRepository,
    IOpenSmartLockInteractor openSmartLockInteractor,
    IGetNukiCredentialInteractor getNukiCredentialInteractor)
  {
    _smartLockKeyRepository = smartLockKeyRepository;
    _openSmartLockInteractor = openSmartLockInteractor;
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

    var openResult = await _openSmartLockInteractor.Handle(
      nukiCredentialResult.Value.UserId,
      smartLockProvider,
      smartLockKeyResult.Value.SmartLockId,
      nukiCredentialResult.Value.Id
    );

    if (openResult.IsFailed)
    {
      return Result.Fail(openResult.Errors);
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