using System.Diagnostics;
using FluentResults;
using Kerbero.Domain.SmartLocks.Dtos;
using Kerbero.Domain.SmartLocks.Interfaces;
using Kerbero.Domain.SmartLocks.Mappers;
using Kerbero.Domain.SmartLocks.Params;
using Kerbero.Domain.SmartLocks.Repositories;

namespace Kerbero.Domain.SmartLocks.Interactors;

public class GetSmartLocksInteractor : IGetSmartLocksInteractor
{
  private readonly INukiSmartLockRepository _nukiSmartLockRepository;

  public GetSmartLocksInteractor(INukiSmartLockRepository nukiSmartLockRepository)
  {
    _nukiSmartLockRepository = nukiSmartLockRepository;
  }

  public async Task<Result<List<SmartLockDto>>> Handle(GetSmartLocksInteractorParams request)
  {
    List<SmartLockDto> smartLockDtos = new();
    foreach (var nukiCredential in request.NukiCredentials)
    {
      var nukiSmartLockResult = await _nukiSmartLockRepository.GetAll(nukiCredential);

      if (nukiSmartLockResult.IsFailed)
      {
        return Result.Fail(nukiSmartLockResult.Errors);
      }

      Debug.Assert(nukiCredential.Id != null, "nukiCredential.Id != null");
      var dtos = SmartLockMapper.Map(nukiSmartLockResult.Value, "nuki", nukiCredential.Id.Value);
      smartLockDtos.AddRange(dtos);
    }

    return smartLockDtos;
  }
}