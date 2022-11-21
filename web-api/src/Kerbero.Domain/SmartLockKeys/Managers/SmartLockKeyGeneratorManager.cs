using FluentResults;
using Kerbero.Domain.SmartLockKeys.Entities;
using Kerbero.Domain.SmartLockKeys.Models.PresentationRequests;

namespace Kerbero.Domain.SmartLockKeys.Managers;

public class SmartLockKeyGeneratorManager: ISmartLockKeyGeneratorManager
{
	public Result<SmartLockKey> GenerateSmartLockKey(int smartLockId, DateTime expiryDate)
	{
		return new SmartLockKey
		{
			Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
			ExpiryDate = expiryDate,
			CreationDate = DateTime.Now,
			IsDisabled = false,
			UsageCounter = 0,
			NukiSmartLockId = smartLockId
		};
	}
}
