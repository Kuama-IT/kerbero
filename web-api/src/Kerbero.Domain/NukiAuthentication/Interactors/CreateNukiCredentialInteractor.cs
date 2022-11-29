using FluentResults;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Mappers;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class CreateNukiCredentialInteractor : ICreateNukiCredentialInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public CreateNukiCredentialInteractor(INukiCredentialRepository nukiCredentialRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<NukiCredentialDto>> Handle(CreateNukiCredentialParams request)
  {
    var nukiCredentialModel = new NukiCredentialModel() { Token = request.Token}; 
    var createNukiCredentialResult = await _nukiCredentialRepository.Create(nukiCredentialModel, request.UserId);

    if (createNukiCredentialResult.IsFailed)
    {
      return Result.Fail(createNukiCredentialResult.Errors);
    }
    return NukiCredentialMapper.Map(createNukiCredentialResult.Value);
  }
}
