using FluentResults;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Mappers;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class CreateNukiAccountInteractor : ICreateNukiAccountInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;
  private readonly INukiOAuthRepository _nukiOAuthRepository;
  private readonly INukiCredentialDraftRepository _nukiCredentialDraftRepository;

  public CreateNukiAccountInteractor(INukiCredentialRepository nukiCredentialRepository,
    INukiOAuthRepository nukiOAuthRepository,
    INukiCredentialDraftRepository nukiCredentialDraftRepository)
  {
    _nukiOAuthRepository = nukiOAuthRepository;
    _nukiCredentialDraftRepository = nukiCredentialDraftRepository;
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<NukiCredentialDto>> Handle(CreateNukiCredentialParams request)
  {
    var nukiAccountDraftResult = await _nukiCredentialDraftRepository.GetByClientId(clientId: request.ClientId);

    if (nukiAccountDraftResult.IsFailed)
    {
      return Result.Fail(nukiAccountDraftResult.Errors);
    }

    await _nukiCredentialDraftRepository.DeleteByClientId(clientId: request.ClientId);

    var nukiCredentialResult = await _nukiOAuthRepository.Authenticate(request.ClientId, request.Code);

    if (nukiCredentialResult.IsFailed)
    {
      return Result.Fail(nukiCredentialResult.Errors);
    }

    var createNukiAccountResult = await _nukiCredentialRepository.Create(nukiCredentialResult.Value);

    if (createNukiAccountResult.IsFailed)
    {
      return Result.Fail(createNukiAccountResult.Errors);
    }

    return NukiCredentialMapper.Map(createNukiAccountResult.Value);
  }
}