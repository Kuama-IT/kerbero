using FluentResults;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Mappers;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class GetNukiCredentialsByUserInteractor : IGetNukiCredentialsByUserInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public GetNukiCredentialsByUserInteractor(INukiCredentialRepository nukiCredentialRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<List<NukiCredentialDto>>> Handle(GetNukiCredentialsByUserInteractorParams request)
  {
    var nukiCredentialsResult = await _nukiCredentialRepository.GetAllByUserId(request.UserId);

    if (nukiCredentialsResult.IsFailed)
    {
      return Result.Fail(nukiCredentialsResult.Errors);
    }

    return NukiCredentialMapper.Map(nukiCredentialsResult.Value);
  }
}
