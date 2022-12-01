using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class ConfirmNukiDraftCredentialsInteractor : IConfirmNukiDraftCredentialsInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;
  private readonly IKerberoConfigurationRepository _kerberoConfigurationRepository;


  public ConfirmNukiDraftCredentialsInteractor(INukiCredentialRepository nukiCredentialRepository,
    IKerberoConfigurationRepository kerberoConfigurationRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
    _kerberoConfigurationRepository = kerberoConfigurationRepository;
  }

  public async Task<Result<NukiCredentialModel>> Handle(string code, Guid draftCredentialUriGuid)
  {
    var nukiApiDefinitionResult = await _kerberoConfigurationRepository.GetApiDefinition();

    if (nukiApiDefinitionResult.IsFailed)
    {
      return Result.Fail(nukiApiDefinitionResult.Errors);
    }

    // Ensure we have the requested draft

    var redirectUri =
      NukiCredentialDraftModel.GetOAuthRedirectUri(nukiApiDefinitionResult.Value, draftCredentialUriGuid);

    var nukiCredentialDraftResult =
      await _nukiCredentialRepository.GetRefreshableCredentialByUrl(redirectUri.ToString());

    if (nukiCredentialDraftResult.IsFailed)
    {
      return Result.Fail(nukiCredentialDraftResult.Errors);
    }

    // Retrieve a token

    var refreshableCredentialResult = await _nukiCredentialRepository.GetRefreshableCredential(code);

    // Confirm the draft
    Result<NukiCredentialModel> nukiCredentialModelResult =
      await _nukiCredentialRepository.ConfirmDraft(nukiCredentialDraftResult.Value, refreshableCredentialResult.Value);

    if (nukiCredentialModelResult.IsFailed)
    {
      return nukiCredentialModelResult.ToResult();
    }

    return nukiCredentialModelResult.Value;
  }
}