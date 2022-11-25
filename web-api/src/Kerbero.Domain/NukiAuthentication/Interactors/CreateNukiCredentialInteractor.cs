using FluentResults;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Mappers;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class CreateNukiCredentialInteractor : ICreateNukiCredentialInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;
  private readonly INukiOAuthRepository _nukiOAuthRepository;
  private readonly INukiCredentialDraftRepository _nukiCredentialDraftRepository;

  public CreateNukiCredentialInteractor(INukiCredentialRepository nukiCredentialRepository,
    INukiOAuthRepository nukiOAuthRepository,
    INukiCredentialDraftRepository nukiCredentialDraftRepository)
  {
    _nukiOAuthRepository = nukiOAuthRepository;
    _nukiCredentialDraftRepository = nukiCredentialDraftRepository;
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<NukiCredentialDto>> Handle(CreateNukiCredentialParams request)
  {
    var nukiCredentialDraftResult = await _nukiCredentialDraftRepository.GetByClientId(clientId: request.ClientId);

    if (nukiCredentialDraftResult.IsFailed)
    {
      return Result.Fail(nukiCredentialDraftResult.Errors);
    }

    var nukiCredentialResult = await _nukiOAuthRepository.Authenticate(request.ClientId, request.Code);

    if (nukiCredentialResult.IsFailed)
    {
      return Result.Fail(nukiCredentialResult.Errors);
    }

    var userId = nukiCredentialDraftResult.Value.UserId;
    var createNukiCredentialResult = await _nukiCredentialRepository.Create(nukiCredentialResult.Value, userId);

    if (createNukiCredentialResult.IsFailed)
    {
      return Result.Fail(createNukiCredentialResult.Errors);
    }

    await _nukiCredentialDraftRepository.DeleteByClientId(clientId: request.ClientId);

    return NukiCredentialMapper.Map(createNukiCredentialResult.Value);
  }
}