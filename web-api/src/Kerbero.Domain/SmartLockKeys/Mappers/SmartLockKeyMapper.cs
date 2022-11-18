using Kerbero.Domain.SmartLockKeys.Entities;
using Kerbero.Domain.SmartLockKeys.Models.PresentationResponses;

namespace Kerbero.Domain.SmartLockKeys.Mappers;

public static class SmartLockKeyMapper
{
	public static CreateSmartLockKeyPresentationResponse Map(SmartLockKey result)
	{
		return new CreateSmartLockKeyPresentationResponse()
		{
			Id = result.Id,
			Token = result.Token,
			CreationDate = result.CreationDate,
			ExpiryDate = result.ExpiryDate
		};
	}
}
