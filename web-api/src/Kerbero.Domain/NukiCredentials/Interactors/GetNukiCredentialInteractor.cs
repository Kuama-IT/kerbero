using FluentResults;
using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Mappers;
using Kerbero.Domain.NukiCredentials.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class GetNukiCredentialInteractor : IGetNukiCredentialInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public GetNukiCredentialInteractor(INukiCredentialRepository nukiCredentialRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<NukiCredentialDto>> Handle(int nukiCredentialId)
  {
    var nukiCredentialsResult = await _nukiCredentialRepository.GetById(nukiCredentialId);

    if (nukiCredentialsResult.IsFailed)
    {
      return Result.Fail(nukiCredentialsResult.Errors);
    }

    return NukiCredentialMapper.Map(nukiCredentialsResult.Value);
  }
}