using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Infrastructure.NukiCredentials.Mappers;
using Kerbero.Infrastructure.SmartLockKeys.Entities;

namespace Kerbero.Infrastructure.SmartLockKeys.Mappers;

public static class SmartLockKeyMapper
{
	public static SmartLockKeyEntity Map(SmartLockKeyModel smartLockKeyModel)
	{
		return new SmartLockKeyEntity
		{
			Token = smartLockKeyModel.Token,
			CreationDate = smartLockKeyModel.CreationDate,
			ExpiryDate = smartLockKeyModel.ExpiryDate,
			IsDisabled = smartLockKeyModel.IsDisabled,
			UsageCounter = smartLockKeyModel.UsageCounter,
			SmartLockId = smartLockKeyModel.SmartLockId,
			CredentialId = smartLockKeyModel.CredentialId
		};
	}

	public static SmartLockKeyModel Map(SmartLockKeyEntity entity)
	{
		return new SmartLockKeyModel()
		{
			Id = entity.Id,
			Token = entity.Token,
			CreationDate = entity.CreationDate,
			ExpiryDate = entity.ExpiryDate,
			IsDisabled = entity.IsDisabled,
			UsageCounter = entity.UsageCounter,
			SmartLockId = entity.SmartLockId,
			CredentialId = entity.CredentialId
		};
	}

	public static List<SmartLockKeyModel> Map(List<SmartLockKeyEntity> entities)
	{
		return entities.ConvertAll(Map);
	}
}
