using FluentAssertions;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.NukiCredentials.Mappers;
using Kerbero.Infrastructure.SmartLockKeys.Mappers;
using Kerbero.Infrastructure.SmartLockKeys.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Infrastructure.Tests.SmartLockKeys.Repositories;

public class SmartLockKeyRepositoryTests
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
			Password = "TOKEN",
			CreationDate = DateTime.Now,
			ExpiryDate = DateTime.Now.AddDays(7),
			UsageCounter = 0,
			IsDisabled = false,
			SmartLockId = "VALID_ID",
			CredentialId = 1,
			SmartLockProvider = SmartLockProvider.Nuki.Name
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

	[Fact]
	public async Task GetAllSmartLockKeyByCredentials_ValidUser()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: "GetAll_Success")
			.Options;
		var applicationDbContext = new ApplicationDbContext(options);
		var smartLockKeyPersistentRepository = new SmartLockKeyRepository(_logger.Object, applicationDbContext);
		
		var tModel = new SmartLockKeyModel
		{
			Password = "TOKEN",
			CreationDate = DateTime.Now,
			ExpiryDate = DateTime.Now.AddDays(7),
			UsageCounter = 0,
			IsDisabled = false,
			SmartLockId = "VALID_ID",
			CredentialId = 1,
			SmartLockProvider = SmartLockProvider.Nuki.Name
		};

		applicationDbContext.SmartLockKeys.Add(SmartLockKeyMapper.Map(tModel));
		await applicationDbContext.SaveChangesAsync();

		var tCredential = new NukiCredentialModel()
		{
			Id = 1,
			Token = "VALID_TOKEN"
		};

		var result = await smartLockKeyPersistentRepository.GetAllByCredentials(new List<NukiCredentialModel>()
		{
			tCredential
		});
		result.IsSuccess.Should().BeTrue();
	}
	
	[Fact]
	public async Task DeleteKey_ValidId()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: "GetAll_Success")
			.Options;
		var applicationDbContext = new ApplicationDbContext(options);
		var smartLockKeyPersistentRepository = new SmartLockKeyRepository(_logger.Object, applicationDbContext);
		
		var tModel = new SmartLockKeyModel
		{
			Password = "TOKEN",
			CreationDate = DateTime.Now,
			ExpiryDate = DateTime.Now.AddDays(7),
			UsageCounter = 0,
			IsDisabled = false,
			SmartLockId = "VALID_ID",
			CredentialId = 1,
			SmartLockProvider = SmartLockProvider.Nuki.Name
		};

		var smartLockKeyEntity = applicationDbContext.SmartLockKeys.Add(SmartLockKeyMapper.Map(tModel));
		await applicationDbContext.SaveChangesAsync();

		var result = await smartLockKeyPersistentRepository.Delete(smartLockKeyEntity.Entity.Id);
		result.IsSuccess.Should().BeTrue();
	}
}
