using System.Text.Json;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Infrastructure.Common.Context;

namespace Kerbero.Integration.Tests;

public static class IntegrationTestsUtils
{
	#region Seed DB

	public static NukiAccount GetSeedingNukiAccount()
	{
		return new NukiAccount()
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			TokenExpiringTimeInSeconds = 2592000,
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
			ExpiryDate = DateTime.Now.AddSeconds(2592000)
		};
	}
	
	public static NukiSmartLock GetSeedingNukiSmartLock()
	{
		return new NukiSmartLock
		{
			Favourite = true,
			Name = "kquarter",
			Type = 0,
			NukiAccountId = 0,
			AuthId = 0,
			ExternalSmartLockId = 0,
			State = new NukiSmartLockState
			{
				Mode = 4,
				State = 255,
				LastAction = 5,
				BatteryCritical = true,
				BatteryCharging = true,
				BatteryCharge = 100,
				DoorState = 255,
				OperationId = "string"
			}
		};
	}

	#endregion
}
