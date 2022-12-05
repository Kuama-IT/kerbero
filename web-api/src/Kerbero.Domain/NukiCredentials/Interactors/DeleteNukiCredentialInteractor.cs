using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class DeleteNukiCredentialInteractor : IDeleteNukiCredentialInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public DeleteNukiCredentialInteractor(
    INukiCredentialRepository nukiCredentialRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<NukiCredentialModel>> Handle(Guid userId, int nukiCredentialId)
  {
    var nukiCredentialResult = await _nukiCredentialRepository.GetById(nukiCredentialId);
    if (nukiCredentialResult.IsFailed)
    {
      return Result.Fail(nukiCredentialResult.Errors);
    }

    if (nukiCredentialResult.Value.UserId != userId)
    {
      return Result.Fail(new UnauthorizedAccessError());
    }

    return await _nukiCredentialRepository.DeleteById(nukiCredentialId);
  }
}