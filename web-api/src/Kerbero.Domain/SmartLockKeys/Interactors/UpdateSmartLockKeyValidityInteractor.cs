using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class UpdateSmartLockKeyValidityInteractor : IUpdateSmartLockKeyValidityInteractor
{
  private readonly ISmartLockKeyRepository _smartLockKeyRepository;
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public UpdateSmartLockKeyValidityInteractor(ISmartLockKeyRepository smartLockKeyRepository,
    INukiCredentialRepository nukiCredentialRepository)
  {
    _smartLockKeyRepository = smartLockKeyRepository;
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<SmartLockKeyModel>> Handle(Guid userId, Guid smartLockKeyGuid, DateTime validUntil,
    DateTime validFrom)
  {
    var smartLockKeyResult = await _smartLockKeyRepository.GetById(smartLockKeyGuid);
    if (smartLockKeyResult.IsFailed)
    {
      return Result.Fail(smartLockKeyResult.Errors);
    }

    var nukiCredentialsResult = await _nukiCredentialRepository.GetAllByUserId(userId);
    if (nukiCredentialsResult.IsFailed)
    {
      return Result.Fail(nukiCredentialsResult.Errors);
    }

    var nukiCredentialModel =
      nukiCredentialsResult.Value.Find(credential => credential.Id == smartLockKeyResult.Value.CredentialId);
    if (nukiCredentialModel is null)
    {
      return Result.Fail(new UnauthorizedAccessError());
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