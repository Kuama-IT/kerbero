using System.Threading.Tasks;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Mappers;
using Kerbero.Domain.SmartLocks.Dtos;
using Kerbero.Domain.SmartLocks.Interfaces;
using Kerbero.Domain.SmartLocks.Repositories;

namespace Kerbero.Domain.SmartLocks.Interactors;

public class OpenSmartLockInteractor : IOpenSmartLockInteractor
{
  private readonly IGetNukiCredentialsByUserInteractor _getNukiCredentialsByUserInteractor;
  private readonly INukiSmartLockRepository _nukiSmartLockRepository;

  public OpenSmartLockInteractor(IGetNukiCredentialsByUserInteractor getNukiCredentialsByUserInteractor,
    INukiSmartLockRepository nukiSmartLockRepository)
  {
    _getNukiCredentialsByUserInteractor = getNukiCredentialsByUserInteractor;
    _nukiSmartLockRepository = nukiSmartLockRepository;
  }

  public async Task<Result> Handle(Guid userId, SmartLockProvider smartLockProvider, string smartLockId, int credentialId)
  {
    var userNukiCredentialsResult =
      await _getNukiCredentialsByUserInteractor.Handle(userId: userId);

    if (userNukiCredentialsResult.IsFailed)
    {
      return Result.Fail(userNukiCredentialsResult.Errors);
    }

    // ensure provided credentials belong to current user
    var credential = userNukiCredentialsResult.Value.Find(it => it.Id == credentialId);

    if (credential is null)
    {
      return Result.Fail(new UnauthorizedAccessError());
    }

    if (smartLockProvider != SmartLockProvider.Nuki)
    {
      return Result.Fail(new UnsupportedSmartLockProviderError());
    }

    var result =
      await _nukiSmartLockRepository.Open(NukiCredentialMapper.Map(credential), smartLockId);

    return result;
  }
}
