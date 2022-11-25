using FluentResults;
using FluentResults.Extensions;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Mappers;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class CreateNukiCredentialDraftInteractor : ICreateNukiCredentialDraftInteractor
{
  private readonly INukiOAuthRepository _nukiOAuthRepository;
  private readonly INukiCredentialDraftRepository _nukiCredentialDraftRepository;

  public CreateNukiCredentialDraftInteractor(
    INukiOAuthRepository nukiOAuthRepository,
    INukiCredentialDraftRepository nukiCredentialDraftRepository)
  {
    _nukiOAuthRepository = nukiOAuthRepository;
    _nukiCredentialDraftRepository = nukiCredentialDraftRepository;
  }

  public async Task<Result<NukiCredentialDraftDto>> Handle(
    CreateNukiCredentialDraftParams createNukiCredentialDraftParams)
  {
    var redirectResult = _nukiOAuthRepository.GetOAuthRedirectUri(createNukiCredentialDraftParams.ClientId);

    if (redirectResult.IsFailed)
    {
      return redirectResult.ToResult();
    }

    // prepare draft credential record
    var nukiCredentialDraft = new NukiCredentialDraft(
      ClientId: createNukiCredentialDraftParams.ClientId,
      UserId: createNukiCredentialDraftParams.UserId,
      RedirectUrl: redirectResult.Value.ToString()
    );

    var createResult = await _nukiCredentialDraftRepository.Create(nukiCredentialDraft);

    if (createResult.IsFailed)
    {
      return Result.Fail(createResult.Errors);
    }

    return NukiCredentialDraftMapper.Map(nukiCredentialDraft);
  }
}