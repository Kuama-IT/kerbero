using FluentResults;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

/// <summary>
/// Nuki Web platform authentication flow:
/// Kerbero User, authenticated on Kerbero
/// -> Redirect to Nuki website
/// -> User accepts / allows Kerbero Nuki app to act in his behalf
/// -> Nuki  website calls Kerbero API
/// -> User comes back to kerbero with a lax cookie
///
/// Nuki Web redirect uri is driven by (<see cref="BuildNukiRedirectUriInteractor"/>
/// </summary>
public class CreateNukiCredentialDraftInteractor : ICreateNukiCredentialDraftInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public CreateNukiCredentialDraftInteractor(INukiCredentialRepository nukiCredentialRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<NukiCredentialDraftModel>> Handle(Guid userId)
  {
    var nukiCredentialDraft = new NukiCredentialDraftModel(UserId: userId);

    var createResult = await _nukiCredentialRepository.CreateDraft(nukiCredentialDraft);

    if (createResult.IsFailed)
    {
      return Result.Fail(createResult.Errors);
    }

    return nukiCredentialDraft;
  }
}