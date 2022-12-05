using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class GetNukiCredentialInteractor : IGetNukiCredentialInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public GetNukiCredentialInteractor(INukiCredentialRepository nukiCredentialRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<NukiCredentialModel>> Handle(int nukiCredentialId, Guid? userId = null)
  {
    var nukiCredentialResult = await _nukiCredentialRepository.GetById(nukiCredentialId);

    if (nukiCredentialResult.IsFailed)
    {
      return Result.Fail(nukiCredentialResult.Errors);
    }

    if (userId is not null && nukiCredentialResult.Value.UserId != userId)
    {
      return Result.Fail(new UnauthorizedAccessError());
    }

    if (nukiCredentialResult.Value.IsRefreshable)
    {
      var refreshedNukiCredentialResult =
        await _nukiCredentialRepository.GetRefreshedCredential(nukiCredentialResult.Value);

      if (nukiCredentialResult.IsFailed)
      {
        return Result.Fail(nukiCredentialResult.Errors);
      }

      nukiCredentialResult = refreshedNukiCredentialResult;
    }

    return nukiCredentialResult.Value;
  }
}