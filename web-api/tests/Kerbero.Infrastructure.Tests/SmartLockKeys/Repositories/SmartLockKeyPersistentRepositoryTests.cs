using FluentAssertions;
using Kerbero.Domain.SmartLockKeys.Entities;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.SmartLockKeys.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Infrastructure.Tests.SmartLockKeys.Repositories;

public class SmartLockKeyPersistentRepositoryTests
{
	private readonly SmartLockKeyPersistentRepository _smartLockKeyPersistentRepository;

	public SmartLockKeyPersistentRepositoryTests()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: "SmartLockTestDbContext")
			.Options;
		var applicationDbContext = new ApplicationDbContext(options);
		var logger = new Mock<ILogger<SmartLockKeyPersistentRepository>>();
		_smartLockKeyPersistentRepository = new SmartLockKeyPersistentRepository(logger.Object, applicationDbContext);
	}

	[Fact]
	public async Task Create_Success()
	{
		var tSmartLockKey = new SmartLockKey
		{
			Token = "TOKEN",
			CreationDate = DateTime.Now,
			ExpiryDate = DateTime.Now.AddDays(7),
			UsageCounter = 0,
			IsDisabled = false,
			NukiSmartLockId = 1,
		};
		var result = await _smartLockKeyPersistentRepository.Create(tSmartLockKey);
		result.IsSuccess.Should().BeTrue();
		tSmartLockKey.Id = result.Value.Id;
		result.Value.Should().Be(tSmartLockKey);
	}

	[Fact]
	public async Task Create_DuplicateEntry_Error()
	{
		var tSmartLockKey = new SmartLockKey
		{
			Token = "TOKEN",
			CreationDate = DateTime.Now,
			ExpiryDate = DateTime.Now.AddDays(7),
			UsageCounter = 0,
			IsDisabled = false,
			NukiSmartLockId = 1,
		};
		var result = await _smartLockKeyPersistentRepository.Create(tSmartLockKey);
		result.IsSuccess.Should().BeTrue();
		result = await _smartLockKeyPersistentRepository.Create(tSmartLockKey);
		result.IsFailed.Should().BeTrue();
	}
}
