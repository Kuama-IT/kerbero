using Kerbero.Domain.SmartLocks.Models;
using Kerbero.WebApi.Dtos;

namespace Kerbero.WebApi.Mappers;

public static class SmartLockMapper
{
  public static List<SmartLockResponseDto> Map(List<SmartLockWithCredentialModel> models)
  {
    return models.ConvertAll(Map);
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
        Value = (int) model.State,
        Description = model.State.ToString()
      }
    };
  }
}