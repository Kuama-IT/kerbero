using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Dtos;
using Kerbero.Domain.SmartLocks.Interfaces;
using Kerbero.Domain.SmartLocks.Mappers;
using Kerbero.Domain.SmartLocks.Repositories;

namespace Kerbero.Domain.SmartLocks.Interactors;

public class GetSmartLocksInteractor : IGetSmartLocksInteractor
{
  private readonly INukiSmartLockRepository _nukiSmartLockRepository;

  public GetSmartLocksInteractor(INukiSmartLockRepository nukiSmartLockRepository)
  {
    _nukiSmartLockRepository = nukiSmartLockRepository;
  }

  public async Task<Result<List<SmartLockDto>>> Handle(List<NukiCredentialModel> nukiCredentials)
  {
    List<SmartLockDto> smartLockDtos = new();
    foreach (var nukiCredential in nukiCredentials)
    {
      var nukiSmartLockResult = await _nukiSmartLockRepository.GetAll(nukiCredential);

      if (nukiSmartLockResult.IsFailed)
      {
        return Result.Fail(nukiSmartLockResult.Errors);
      }

      var dtos = SmartLockMapper.Map(nukiSmartLockResult.Value, nukiCredential.Id);
      smartLockDtos.AddRange(dtos);
    }

    return smartLockDtos;
  }
}
