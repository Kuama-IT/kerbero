using FluentResults;
using Kerbero.Domain.NukiCredentials.Errors;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class CreateNukiCredentialInteractor : ICreateNukiCredentialInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public CreateNukiCredentialInteractor(
    INukiCredentialRepository nukiCredentialRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<NukiCredentialModel>> Handle(Guid userId, string token)
  {
    var result = await _nukiCredentialRepository.ValidateNotRefreshableApiToken(token);
    if (result.IsFailed)
    {
      return Result.Fail(new NukiCredentialInvalidTokenError());
    }

    var nukiCredentialModel = new NukiCredentialModel() { Token = token };
    var createNukiCredentialResult = await _nukiCredentialRepository.Create(nukiCredentialModel, userId);

    if (createNukiCredentialResult.IsFailed)
    {
      return Result.Fail(createNukiCredentialResult.Errors);
    }

    return createNukiCredentialResult.Value;
  }
}