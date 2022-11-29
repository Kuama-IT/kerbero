using FluentResults;
using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Errors;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Mappers;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLocks.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class CreateNukiCredentialInteractor : ICreateNukiCredentialInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public CreateNukiCredentialInteractor(
    INukiCredentialRepository nukiCredentialRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<NukiCredentialDto>> Handle(CreateNukiCredentialParams request)
  {
    var result = await _nukiCredentialRepository.ValidateApiToken(request.Token);
    if (result.IsFailed)
    {
      return Result.Fail(new NukiCredentialInvalidTokenError());
    }

    var nukiCredentialModel = new NukiCredentialModel() { Token = request.Token };
    var createNukiCredentialResult = await _nukiCredentialRepository.Create(nukiCredentialModel, request.UserId);

    if (createNukiCredentialResult.IsFailed)
    {
      return Result.Fail(createNukiCredentialResult.Errors);
    }

    return NukiCredentialMapper.Map(createNukiCredentialResult.Value);
  }
}