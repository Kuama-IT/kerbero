using Kerbero.Domain.SmartLocks.Models;
using Kerbero.Infrastructure.SmartLocks.Models;

namespace Kerbero.Infrastructure.SmartLocks.Mappers;

public static class SmartLockMapper
{
  public static List<SmartLock> Map(List<NukiSmartLockResponse> responses)
  {
    return responses.ConvertAll(Map);
  }
  
  public static SmartLock Map(NukiSmartLockResponse response)
  {
    return new SmartLock
    {
      Id = response.NukiAccountId.ToString(),
      Name = response.Name
    };
  }
}