using Kerbero.Domain.SmartLockKeys.Dtos;
using Kerbero.Domain.SmartLockKeys.Models;

namespace Kerbero.Domain.SmartLockKeys.Mappers;

public static class SmartLockKeyMapper
{
	public static SmartLockKeyDto Map(SmartLockKeyModel result)
	{
		return new SmartLockKeyDto()
		{
			Id = result.Id,
			Password = result.Password,
			CreationDate = result.CreationDate,
			ExpiryDate = result.ExpiryDate
		};
	}	
	
	public static List<SmartLockKeyDto> Map(List<SmartLockKeyModel> models)
	{
		return models.ConvertAll(Map);
	}
}
