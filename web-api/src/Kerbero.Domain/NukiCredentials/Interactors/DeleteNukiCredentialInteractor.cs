using FluentResults;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class DeleteNukiCredentialInteractor : IDeleteNukiCredentialInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;
  private readonly IEnsureNukiCredentialBelongsToUserInteractor _ensureNukiCredentialBelongsToUserInteractor;

  public DeleteNukiCredentialInteractor(
    INukiCredentialRepository nukiCredentialRepository,
    IEnsureNukiCredentialBelongsToUserInteractor ensureNukiCredentialBelongsToUserInteractor)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
    _ensureNukiCredentialBelongsToUserInteractor = ensureNukiCredentialBelongsToUserInteractor;
  }

  public async Task<Result<NukiCredentialModel>> Handle(Guid userId, int nukiCredentialId)
  {
    var result = await _ensureNukiCredentialBelongsToUserInteractor.Handle(userId, nukiCredentialId);
    if (result.IsFailed)
    {
      return result.ToResult();
    }

    return await _nukiCredentialRepository.DeleteById(nukiCredentialId);
  }
}