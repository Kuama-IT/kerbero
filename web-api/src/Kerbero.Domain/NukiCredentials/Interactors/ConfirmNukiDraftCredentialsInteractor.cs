using FluentResults;
using Kerbero.Domain.Common.Repositories;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.NukiCredentials.Utils;

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

  public async Task<Result<NukiCredentialModel>> Handle(string code, Guid userId)
  {
    var nukiApiDefinitionResult = await _kerberoConfigurationRepository.GetNukiApiDefinition();
    
    if (nukiApiDefinitionResult.IsFailed)
    {
      return Result.Fail(nukiApiDefinitionResult.Errors);
    }

    var applicationRedirectUri = BuildRedirectToKerberoUriHelper.Handle(nukiApiDefinitionResult.Value);

    // Ensure we have the requested draft
    var nukiCredentialDraftResult = 
      await _nukiCredentialRepository.GetDraftCredentialsByUserId(userId);

    if (nukiCredentialDraftResult.IsFailed)
    {
      return Result.Fail(nukiCredentialDraftResult.Errors);
    }

    // Retrieve a token
    var refreshableCredentialResult = await _nukiCredentialRepository.GetRefreshableCredential(code, applicationRedirectUri);

    // Confirm the draft
    var nukiCredentialModelResult =
      await _nukiCredentialRepository.ConfirmDraft(nukiCredentialDraftResult.Value, refreshableCredentialResult.Value);

    if (nukiCredentialModelResult.IsFailed)
    {
      return nukiCredentialModelResult.ToResult();
    }

    // clear any pending draft
    await _nukiCredentialRepository.DeleteDraftByUserId(userId);

    return nukiCredentialModelResult.Value;
  }
}
