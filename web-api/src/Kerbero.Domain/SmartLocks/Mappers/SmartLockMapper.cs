using Kerbero.Domain.SmartLocks.Dtos;
using Kerbero.Domain.SmartLocks.Models;

namespace Kerbero.Domain.SmartLocks.Mappers;

public static class SmartLockMapper
{
  public static List<SmartLockDto> Map(List<SmartLock> models, string provider, int credentialId)
  {
    return models.ConvertAll(e => Map(e, provider, credentialId));
  }

  public static SmartLockDto Map(SmartLock model, string provider, int credentialId)
  {
    return new SmartLockDto
    {
      Id = model.Id,
      Name = model.Name,
      Provider = provider,
      CredentialId = credentialId,
    };
  }
}