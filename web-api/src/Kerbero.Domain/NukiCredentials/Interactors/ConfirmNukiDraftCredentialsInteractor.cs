using FluentResults;
using Kerbero.Domain.Common.Repositories;
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

  public async Task<Result<NukiCredentialModel>> Handle(string code, Guid userId)
  {
    var buildNukiRedirectInteractor = new BuildNukiRedirectUriInteractor(_kerberoConfigurationRepository);
    var redirectUriResult = await buildNukiRedirectInteractor.Handle();
    
    if (redirectUriResult.IsFailed)
    {
      return Result.Fail(redirectUriResult.Errors);
    }

    // Ensure we have the requested draft
    var nukiCredentialDraftResult =
      await _nukiCredentialRepository.GetDraftCredentialsByUserId(userId);

    if (nukiCredentialDraftResult.IsFailed)
    {
      return Result.Fail(nukiCredentialDraftResult.Errors);
    }

    // Retrieve a token
    var refreshableCredentialResult = await _nukiCredentialRepository.GetRefreshableCredential(code, redirectUriResult.Value.ToString());

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