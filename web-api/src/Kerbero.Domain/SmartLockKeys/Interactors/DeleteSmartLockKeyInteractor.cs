using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class DeleteSmartLockKeyInteractor : IDeleteSmartLockKeyInteractor
{
  private readonly ISmartLockKeyRepository _smartLockKeyRepository;
  private readonly IEnsureNukiCredentialBelongsToUserInteractor _ensureNukiCredentialBelongsToUserInteractor;

  public DeleteSmartLockKeyInteractor(
    ISmartLockKeyRepository smartLockKeyRepository,
    IEnsureNukiCredentialBelongsToUserInteractor ensureNukiCredentialBelongsToUserInteractor)
  {
    _smartLockKeyRepository = smartLockKeyRepository;
    _ensureNukiCredentialBelongsToUserInteractor = ensureNukiCredentialBelongsToUserInteractor;
  }

  public async Task<Result<SmartLockKeyModel>> Handle(Guid userId, Guid smartLockId)
  {
    var smartLockKeyResult = await _smartLockKeyRepository.GetById(smartLockId);
    if (smartLockKeyResult.IsFailed)
    {
      return Result.Fail(smartLockKeyResult.Errors);
    }

    var nukiCredentialModelResult =
      await _ensureNukiCredentialBelongsToUserInteractor.Handle(userId,
        credentialId: smartLockKeyResult.Value.CredentialId);
    if (nukiCredentialModelResult.IsFailed)
    {
      return nukiCredentialModelResult.ToResult();
    }

    return await _smartLockKeyRepository.Delete(smartLockId);
  }
}