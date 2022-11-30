using FluentResults;
using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Mappers;
using Kerbero.Domain.NukiCredentials.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class GetNukiCredentialsByUserInteractor : IGetNukiCredentialsByUserInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public GetNukiCredentialsByUserInteractor(INukiCredentialRepository nukiCredentialRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<List<NukiCredentialDto>>> Handle(Guid userId)
  {
    var nukiCredentialsResult = await _nukiCredentialRepository.GetAllByUserId(userId);

    if (nukiCredentialsResult.IsFailed)
    {
      return Result.Fail(nukiCredentialsResult.Errors);
    }

    return NukiCredentialMapper.Map(nukiCredentialsResult.Value);
  }
}
