using FluentResults;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Interfaces;
using Kerbero.Domain.SmartLocks.Models;
using Kerbero.Domain.SmartLocks.Repositories;

namespace Kerbero.Domain.SmartLocks.Interactors;

public class GetSmartLocksInteractor : IGetSmartLocksInteractor
{
  private readonly INukiSmartLockRepository _nukiSmartLockRepository;

  public GetSmartLocksInteractor(INukiSmartLockRepository nukiSmartLockRepository)
  {
    _nukiSmartLockRepository = nukiSmartLockRepository;
  }

  public async Task<Result<List<SmartLockWithCredentialModel>>> Handle(List<NukiCredentialModel> nukiCredentials)
  {
    List<SmartLockWithCredentialModel> smartLockDtos = new();
    foreach (var nukiCredential in nukiCredentials)
    {
      var nukiSmartLockResult = await _nukiSmartLockRepository.GetAll(nukiCredential);

      if (nukiSmartLockResult.IsFailed)
      {
        return Result.Fail(nukiSmartLockResult.Errors);
      }

      var dtos = nukiSmartLockResult.Value.ConvertAll(s => new SmartLockWithCredentialModel
      {
        Id = s.Id,
        Name = s.Name,
        SmartLockProvider = SmartLockProvider.Nuki,
        State = s.State,
        CredentialId = nukiCredential.Id
      });
      smartLockDtos.AddRange(dtos);
    }

    return smartLockDtos;
  }
}