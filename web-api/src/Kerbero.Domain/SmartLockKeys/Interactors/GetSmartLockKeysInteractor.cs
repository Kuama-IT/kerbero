using FluentResults;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class GetSmartLockKeysInteractor : IGetSmartLockKeysInteractor
{
  private readonly ISmartLockKeyRepository _smartLockKeyRepository;
  private readonly IGetNukiCredentialsByUserInteractor _getNukiCredentialsByUserInteractor;

  public GetSmartLockKeysInteractor(
    ISmartLockKeyRepository smartLockKeyRepository,
    IGetNukiCredentialsByUserInteractor getNukiCredentialsByUserInteractor)
  {
    _smartLockKeyRepository = smartLockKeyRepository;
    _getNukiCredentialsByUserInteractor = getNukiCredentialsByUserInteractor;
  }

  public async Task<Result<UserSmartLockKeysModel>> Handle(Guid userId)
  {
    var credentialsResult = await _getNukiCredentialsByUserInteractor.Handle(userId);
    if (credentialsResult.IsFailed)
    {
      return Result.Fail(credentialsResult.Errors);
    }

    var smartLockKeysResult =
      await _smartLockKeyRepository.GetAllByCredentials(credentialsResult.Value.NukiCredentials);
    if (smartLockKeysResult.IsFailed)
    {
      return Result.Fail(smartLockKeysResult.Errors);
    }

    return new UserSmartLockKeysModel(
      SmartLockKeys: smartLockKeysResult.Value,
      OutdatedCredentials: credentialsResult.Value.OutdatedCredentials
    );
  }
}