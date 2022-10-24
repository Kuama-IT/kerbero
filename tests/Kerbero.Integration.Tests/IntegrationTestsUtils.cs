using System.Text.Json;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Infrastructure.Common.Context;

namespace Kerbero.Integration.Tests;

public static class IntegrationTestsUtils
{
	#region Seed DB

	public static void InitializeDbForTests(ApplicationDbContext db)
	{
		db.NukiAccounts.AddRange(GetSeedingMessages());
		db.SaveChanges();
	}
	
	private static IEnumerable<NukiAccount> GetSeedingMessages()
	{
		return new List<NukiAccount>()
		{
			new() {
				Token = "VALID_TOKEN",
				RefreshToken = "VALID_REFRESH_TOKEN",
				TokenExpiringTimeInSeconds = 2592000,
				ClientId = "VALID_CLIENT_ID",
				TokenType = "bearer",
				ExpiryDate = DateTime.Now.AddSeconds(2592000)
			},
			new()
			{
				Token = "VALID_TOKEN",
				RefreshToken = "VALID_REFRESH_TOKEN",
				TokenExpiringTimeInSeconds = 2592000,
				ClientId = "VALID_CLIENT_ID",
				TokenType = "bearer",
				ExpiryDate = DateTime.Now.AddSeconds(-2592000)
			}
		};
	}

	#endregion
}
