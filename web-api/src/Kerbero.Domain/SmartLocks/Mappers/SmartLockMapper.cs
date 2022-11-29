using Kerbero.Domain.SmartLocks.Dtos;
using Kerbero.Domain.SmartLocks.Models;

namespace Kerbero.Domain.SmartLocks.Mappers;

public static class SmartLockMapper
{
  public static List<SmartLockDto> Map(List<SmartLock> models, int credentialId)
  {
    return models.ConvertAll(e => Map(e, credentialId));
  }

  public static SmartLockDto Map(SmartLock model, int credentialId)
  {
    return new SmartLockDto
    {
      Id = model.Id,
      Name = model.Name,
      Provider = model.Provider.Name,
      CredentialId = credentialId,
      State = new SmartLockStateDto()
      {
        Value = (int) model.State,
        Description = model.State.ToString()
      }
    };
  }
}