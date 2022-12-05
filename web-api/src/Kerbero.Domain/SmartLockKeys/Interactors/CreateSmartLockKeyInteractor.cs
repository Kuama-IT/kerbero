using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Domain.SmartLocks.Repositories;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class CreateSmartLockKeyInteractor : ICreateSmartLockKeyInteractor
{
  private readonly ISmartLockKeyRepository _smartLockKeyRepository;
  private readonly INukiSmartLockRepository _nukiSmartLockRepository;
  private readonly IGetNukiCredentialInteractor _getNukiCredentialInteractor;

  public CreateSmartLockKeyInteractor(
    ISmartLockKeyRepository smartLockKeyRepository,
    INukiSmartLockRepository nukiSmartLockRepository,
    IGetNukiCredentialInteractor getNukiCredentialInteractor)
  {
    _smartLockKeyRepository = smartLockKeyRepository;
    _nukiSmartLockRepository = nukiSmartLockRepository;
    _getNukiCredentialInteractor = getNukiCredentialInteractor;
  }

  public async Task<Result<SmartLockKeyModel>> Handle(string smartLockId, DateTime validUntilDate,
    DateTime validFromDate,
    int credentialId, SmartLockProvider smartLockProvider)
  {
    if (smartLockProvider != SmartLockProvider.Nuki)
    {
      return Result.Fail(new UnsupportedSmartLockProviderError());
    }

    var nukiCredentialResult = await _getNukiCredentialInteractor.Handle(credentialId);
    if (nukiCredentialResult.IsFailed)
    {
      return Result.Fail(nukiCredentialResult.Errors);
    }

    var nukiSmartLockResult = await _nukiSmartLockRepository.Get(nukiCredentialResult.Value, smartLockId);

    if (nukiSmartLockResult.IsFailed)
    {
      return Result.Fail(nukiCredentialResult.Errors);
    }

    var model =
      SmartLockKeyModel.Create(smartLockId, validUntilDate, validFromDate, credentialId, smartLockProvider);

    var validationResult = model.Validate();
    if (validationResult.IsFailed)
    {
      return Result.Fail(validationResult.Errors);
    }

    var createSmartLockKeyResult = await _smartLockKeyRepository.Create(model);

    if (createSmartLockKeyResult.IsFailed)
    {
      return Result.Fail(createSmartLockKeyResult.Errors);
    }

    return createSmartLockKeyResult.Value;
  }
}