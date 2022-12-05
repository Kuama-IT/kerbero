using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Models;
using Kerbero.WebApi.Dtos.SmartLocks;

namespace Kerbero.WebApi.Mappers;

public static class SmartLockMapper
{
  public static SmartLockListResponseDto Map(List<SmartLockWithCredentialModel> models,
    List<(NukiCredentialModel, List<IError>)> outdatedCredentials)
  {
    return new SmartLockListResponseDto(
      SmartLocks: models.ConvertAll(Map),
      OutdatedCredentials: NukiCredentialMapper.Map(outdatedCredentials)
    );
  }

  public static SmartLockResponseDto Map(SmartLockWithCredentialModel model)
  {
    return new SmartLockResponseDto
    {
      Id = model.Id,
      Name = model.Name,
      SmartLockProvider = model.SmartLockProvider.Name,
      CredentialId = model.CredentialId,
      State = new SmartLockStateDto()
      {
        Value = (int)model.State,
        Description = model.State.ToString()
      }
    };
  }
}