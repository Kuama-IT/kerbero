using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.SmartLocks.Interfaces;
using Kerbero.Domain.SmartLocks.Repositories;

namespace Kerbero.Domain.SmartLocks.Interactors;

public class OpenSmartLockInteractor : IOpenSmartLockInteractor
{
  private readonly IGetNukiCredentialsByUserInteractor _getNukiCredentialsByUserInteractor;
  private readonly INukiSmartLockRepository _nukiSmartLockRepository;
  private readonly IGetNukiCredentialInteractor _getNukiCredentialInteractor;

  public OpenSmartLockInteractor(
    IGetNukiCredentialsByUserInteractor getNukiCredentialsByUserInteractor,
    INukiSmartLockRepository nukiSmartLockRepository,
    IGetNukiCredentialInteractor getNukiCredentialInteractor)
  {
    _getNukiCredentialsByUserInteractor = getNukiCredentialsByUserInteractor;
    _nukiSmartLockRepository = nukiSmartLockRepository;
    _getNukiCredentialInteractor = getNukiCredentialInteractor;
  }

  public async Task<Result> Handle(Guid userId, SmartLockProvider smartLockProvider, string smartLockId,
    int credentialId)
  {
    var credentialResult = await _getNukiCredentialInteractor.Handle(credentialId, userId);

    if (credentialResult.IsFailed)
    {
      return credentialResult.ToResult();
    }

    if (smartLockProvider != SmartLockProvider.Nuki)
    {
      return Result.Fail(new UnsupportedSmartLockProviderError());
    }

    var result =
      await _nukiSmartLockRepository.Open(credentialResult.Value, smartLockId);

    return result;
  }
}