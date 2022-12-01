using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Infrastructure.SmartLockKeys.Entities;

namespace Kerbero.Infrastructure.SmartLockKeys.Mappers;

public static class SmartLockKeyMapper
{
	public static SmartLockKeyEntity Map(SmartLockKeyModel smartLockKeyModel)
	{
		return new SmartLockKeyEntity
		{
			Password = smartLockKeyModel.Password,
			CreationDate = smartLockKeyModel.CreationDate,
			ExpiryDate = smartLockKeyModel.ExpiryDate,
			IsDisabled = smartLockKeyModel.IsDisabled,
			UsageCounter = smartLockKeyModel.UsageCounter,
			SmartLockId = smartLockKeyModel.SmartLockId,
			CredentialId = smartLockKeyModel.CredentialId,
			SmartLockProvider = smartLockKeyModel.SmartLockProvider
		};
	}

	public static SmartLockKeyModel Map(SmartLockKeyEntity entity)
	{
		return new SmartLockKeyModel()
		{
			Id = entity.Id,
			Password = entity.Password,
			CreationDate = entity.CreationDate,
			ExpiryDate = entity.ExpiryDate,
			IsDisabled = entity.IsDisabled,
			UsageCounter = entity.UsageCounter,
			SmartLockId = entity.SmartLockId,
			CredentialId = entity.CredentialId,
			SmartLockProvider = entity.SmartLockProvider
		};
	}

	public static List<SmartLockKeyModel> Map(List<SmartLockKeyEntity> entities)
	{
		return entities.ConvertAll(Map);
	}

	public static void Map(SmartLockKeyEntity entity, SmartLockKeyModel model)
	{
		entity.Password = model.Password;
		entity.CreationDate = model.CreationDate;
		entity.ExpiryDate = model.ExpiryDate;
		entity.IsDisabled = model.IsDisabled;
		entity.UsageCounter = model.UsageCounter;
		entity.SmartLockId = model.SmartLockId;
		entity.CredentialId = model.CredentialId;
		entity.SmartLockProvider = model.SmartLockProvider;
	}
}
