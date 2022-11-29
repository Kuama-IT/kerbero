using System.Threading.Tasks;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Models.ExternalRequests;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Interactors;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Mappers;
using Kerbero.Domain.SmartLocks.Dtos;
using Kerbero.Domain.SmartLocks.Interfaces;
using Kerbero.Domain.SmartLocks.Models;
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

  public async Task<Result> Handle(OpenSmartLockParams openSmartLockParams)
  {
    var userNukiCredentialsResult = await _getNukiCredentialsByUserInteractor.Handle(
      new GetNukiCredentialsByUserInteractorParams
      {
        UserId = openSmartLockParams.UserId,
      });

    if (userNukiCredentialsResult.IsFailed)
    {
      return Result.Fail(userNukiCredentialsResult.Errors);
    }

    // ensure provided credentials belong to current user
    var credential = userNukiCredentialsResult.Value.Find(it => it.Id == openSmartLockParams.CredentialId);

    if (credential is null)
    {
      return Result.Fail(new UnauthorizedAccessError());
    }

    if (openSmartLockParams.SmartLockProvider != SmartLockProvider.Nuki)
    {
      return Result.Fail(new UnsupportedSmartLockProviderError());
    }

    var result =
      await _nukiSmartLockRepository.Open(NukiCredentialMapper.Map(credential), openSmartLockParams.SmartLockId);

    return result;
  }
}