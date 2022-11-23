using FluentAssertions;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.NukiCredentials.Mappers;
using Kerbero.Infrastructure.SmartLockKeys.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Infrastructure.Tests.SmartLockKeys.Repositories;

public class SmartLockKeyPersistentRepositoryTests
{
	private readonly Mock<ILogger<SmartLockKeyRepository>> _logger = new();

	[Fact]
	public async Task CreateSmartLockKey_ValidParameters()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: "CreateSmartLockKey_Success")
			.Options;
		var applicationDbContext = new ApplicationDbContext(options);
		var smartLockKeyPersistentRepository = new SmartLockKeyRepository(_logger.Object, applicationDbContext);
		
		var tSmartLockKey = new SmartLockKeyModel
		{
			Token = "TOKEN",
			CreationDate = DateTime.Now.ToUniversalTime(),
			ExpiryDate = DateTime.Now.AddDays(7).ToUniversalTime(),
			UsageCounter = 0,
			IsDisabled = false,
			SmartLockId = "VALID_ID",
			CredentialId = 1
		};

		var tNukiCredential = new NukiCredentialModel
		{
			Id = 1,
			Token = "VALID_TOKEN"
		};

		applicationDbContext.NukiCredentials.Add(NukiCredentialMapper.Map(tNukiCredential));
		
		var result = await smartLockKeyPersistentRepository.Create(tSmartLockKey);
		result.IsSuccess.Should().BeTrue();
		tSmartLockKey.Id = result.Value.Id;
		result.Value.Should().BeEquivalentTo(tSmartLockKey);
	}
}
