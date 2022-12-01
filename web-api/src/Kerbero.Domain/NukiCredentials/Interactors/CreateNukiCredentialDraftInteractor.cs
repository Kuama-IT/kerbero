using FluentResults;
using Kerbero.Domain.Common.Interfaces;
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
///
/// In the last step of the flow, we do not know which Kerbero user initiated the flow, since Nuki website calls us
/// without any authentication.
///
/// This interactor solves this problem by creating a draft of the user Nuki Credentials, that will be confirmed when
/// Nuki Web redirects the user to us with a matching uri (<see cref="NukiCredentialDraftModel.GetOAuthRedirectUri"/>
/// </summary>
public class CreateNukiCredentialDraftInteractor : ICreateNukiCredentialDraftInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;
  private readonly IKerberoConfigurationRepository _kerberoConfigurationRepository;

  public CreateNukiCredentialDraftInteractor(INukiCredentialRepository nukiCredentialRepository, IKerberoConfigurationRepository kerberoConfigurationRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
    _kerberoConfigurationRepository = kerberoConfigurationRepository;
  }

  public async Task<Result<NukiCredentialDraftModel>> Handle(Guid userId)
  {
    var nukiApiDefinitionResult = await _kerberoConfigurationRepository.GetApiDefinition();
    
    if (nukiApiDefinitionResult.IsFailed)
    {
      return Result.Fail(nukiApiDefinitionResult.Errors);
    }
    
    var redirectUri = NukiCredentialDraftModel.GetOAuthRedirectUri(nukiApiDefinitionResult.Value);
    
    var nukiCredentialDraft = new NukiCredentialDraftModel(
      UserId: userId,
      RedirectUrl: redirectUri.ToString()
    );

    var createResult = await _nukiCredentialRepository.CreateDraft(nukiCredentialDraft);

    if (createResult.IsFailed)
    {
      return Result.Fail(createResult.Errors);
    }

    return nukiCredentialDraft;
  }
}