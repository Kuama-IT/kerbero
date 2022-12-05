using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.WebApi.Dtos;

namespace Kerbero.WebApi.Mappers;

public static class SmartLockKeyMapper
{
  public static SmartLockKeyResponseDto Map(SmartLockKeyModel result)
  {
    return new SmartLockKeyResponseDto()
    {
      Id = result.Id,
      Password = result.Password,
      ValidUntilDate = result.ValidUntil,
      ValidFromDate = result.ValidFrom,
    };
  }

  public static List<SmartLockKeyResponseDto> Map(List<SmartLockKeyModel> models)
  {
    return models.ConvertAll(Map);
  }
}