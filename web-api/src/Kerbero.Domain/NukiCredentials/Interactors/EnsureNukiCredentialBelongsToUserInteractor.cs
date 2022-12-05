using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class EnsureNukiCredentialBelongsToUserInteractor : IEnsureNukiCredentialBelongsToUserInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public EnsureNukiCredentialBelongsToUserInteractor(INukiCredentialRepository nukiCredentialRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<NukiCredentialModel>> Handle(Guid userId, int credentialId)
  {
    var nukiCredentialsResult = await _nukiCredentialRepository.GetAllByUserId(userId);
    if (nukiCredentialsResult.IsFailed)
    {
      return Result.Fail(nukiCredentialsResult.Errors);
    }

    var nukiCredentialModel =
      nukiCredentialsResult.Value.Find(credential => credential.Id == credentialId);
    if (nukiCredentialModel is null)
    {
      return Result.Fail(new UnauthorizedAccessError());
    }

    return nukiCredentialModel;
  }
}