using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class UpdateSmartLockKeyValidityInteractor : IUpdateSmartLockKeyValidityInteractor
{
  private readonly ISmartLockKeyRepository _smartLockKeyRepository;
  private readonly IEnsureNukiCredentialBelongsToUserInteractor _ensureNukiCredentialBelongsToUserInteractor;

  public UpdateSmartLockKeyValidityInteractor(
    ISmartLockKeyRepository smartLockKeyRepository,
    IEnsureNukiCredentialBelongsToUserInteractor ensureNukiCredentialBelongsToUserInteractor)
  {
    _smartLockKeyRepository = smartLockKeyRepository;
    _ensureNukiCredentialBelongsToUserInteractor = ensureNukiCredentialBelongsToUserInteractor;
  }

  public async Task<Result<SmartLockKeyModel>> Handle(Guid userId, Guid smartLockKeyGuid, DateTime validUntil,
    DateTime validFrom)
  {
    var smartLockKeyResult = await _smartLockKeyRepository.GetById(smartLockKeyGuid);
    if (smartLockKeyResult.IsFailed)
    {
      return Result.Fail(smartLockKeyResult.Errors);
    }

    var nukiCredentialsResult =
      await _ensureNukiCredentialBelongsToUserInteractor.Handle(userId, smartLockKeyResult.Value.CredentialId);
    if (nukiCredentialsResult.IsFailed)
    {
      return nukiCredentialsResult.ToResult();
    }

    var model = smartLockKeyResult.Value;
    model.ValidFrom = validFrom;
    model.ValidUntil = validUntil;

    var validationResult = model.Validate();
    if (validationResult.IsFailed)
    {
      return Result.Fail(validationResult.Errors);
    }

    return await _smartLockKeyRepository.Update(model);
  }
}