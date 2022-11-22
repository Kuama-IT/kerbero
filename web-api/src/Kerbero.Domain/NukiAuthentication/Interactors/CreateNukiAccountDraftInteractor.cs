using FluentResults;
using FluentResults.Extensions;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Mappers;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class CreateNukiAccountDraftInteractor : ICreateNukiAccountDraftInteractor
{
  private readonly INukiOAuthRepository _nukiOAuthRepository;
  private readonly INukiCredentialRepository _nukiCredentialRepository;
  private readonly INukiCredentialDraftRepository _nukiCredentialDraftRepository;

  public CreateNukiAccountDraftInteractor(INukiOAuthRepository nukiOAuthRepository,
    INukiCredentialRepository nukiCredentialRepository, INukiCredentialDraftRepository nukiCredentialDraftRepository)
  {
    _nukiOAuthRepository = nukiOAuthRepository;
    _nukiCredentialRepository = nukiCredentialRepository;
    _nukiCredentialDraftRepository = nukiCredentialDraftRepository;
  }

  public async Task<Result<NukiAccountDraftDto>> Handle(CreateNukiAccountDraftParams createNukiAccountDraftParams)
  {
    var redirectResult = _nukiOAuthRepository.GetOAuthRedirectUri(createNukiAccountDraftParams.ClientId);

    if (redirectResult.IsFailed)
    {
      return redirectResult.ToResult();
    }

    // prepare draft account record
    var nukiAccountDraft = new NukiCredentialDraft(
      ClientId: createNukiAccountDraftParams.ClientId,
      UserId: createNukiAccountDraftParams.UserId,
      RedirectUrl: redirectResult.Value.ToString()
    );

    var createResult = await _nukiCredentialDraftRepository.Create(nukiAccountDraft);

    if (createResult.IsFailed)
    {
      return Result.Fail(createResult.Errors);
    }

    return NukiCredentialDraftMapper.Map(nukiAccountDraft);
  }
}