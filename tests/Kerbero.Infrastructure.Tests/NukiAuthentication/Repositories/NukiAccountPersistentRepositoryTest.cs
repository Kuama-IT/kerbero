using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.NukiAuthentication.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Infrastructure.Tests.NukiAuthentication.Repositories;

public class NukiAccountPersistentRepositoryTest: IDisposable
{
	private readonly NukiAccountPersistentRepository _repository;
	private readonly ApplicationDbContext _dbContext;

	public NukiAccountPersistentRepositoryTest()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: "AppDbContext")
			.Options;
		var logger = new Mock<ILogger<NukiAccountPersistentRepository>>();
		_dbContext = new ApplicationDbContext(options);
		_repository = new NukiAccountPersistentRepository(_dbContext, logger.Object);
	}
	
	public void Dispose()
	{
		_dbContext.Dispose();
	}

	#region CreateNukiAccount
	
	[Fact]
	public async Task CreateNukiAccount_Success_Test()
	{
		// Arrange
		var nukiAccount = new NukiAccount
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			TokenExpiringTimeInSeconds = 2592000,
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
		};
		
		var res = await CreateHelper(nukiAccount);
		
		// Assert
		nukiAccount.Id = 1;
		res.Value.Should().BeEquivalentTo(nukiAccount);
	}

	private async Task<Result<NukiAccount>> CreateHelper(NukiAccount nukiAccount)
	{
		_repository.Should().BeAssignableTo<INukiAccountPersistentRepository>();

		// Act
		return await _repository.Create(nukiAccount);
	}
	#endregion

	#region GetNukiAccount

	[Fact]
	public void GetNukiAccount_Success_Test()
	{
		// Arrange
		for (var i = 1; i < 3; i++)
		{
			var nukiAccount = new NukiAccount
			{
				Token = "VALID_TOKEN" + i,
				RefreshToken = "VALID_REFRESH_TOKEN",
				TokenExpiringTimeInSeconds = 2592000,
				ClientId = "VALID_CLIENT_ID" + i,
				TokenType = "bearer",
			};
			_dbContext.NukiAccounts.Add(nukiAccount);
		}

		_dbContext.SaveChanges();
		
		_repository.Should().BeAssignableTo<INukiAccountPersistentRepository>();

		// Act
		var res = _repository.GetAccount(1);
		
		// Assert
		res.Value.Should().BeEquivalentTo(new NukiAccount
		{
			Id = 1,
			Token = "VALID_TOKEN1",
			RefreshToken = "VALID_REFRESH_TOKEN",
			TokenExpiringTimeInSeconds = 2592000,
			ClientId = "VALID_CLIENT_ID1",
			TokenType = "bearer",
		});
	}

	[Fact]
	public void GetNukiAccount_NotValidProviderAccountFoundError_Test()
	{
		// Arrange

		// Act
		var res = _repository.GetAccount(0);
	
		// Assert
		res.IsFailed.Should().BeTrue();
		res.Errors.First().Should().BeEquivalentTo(new UnauthorizedAccessError());
	}

	#endregion
	
	#region UpdateNukiAccount
	
	[Fact]
	public async Task Update_Success_Test()
	{
		// Arrange
		_repository.Should().BeAssignableTo<INukiAccountPersistentRepository>();
		var nukiAccount = new NukiAccount
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			TokenExpiringTimeInSeconds = 2592000,
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
		};
		await CreateHelper(nukiAccount);

		// Act
		var res = await _repository.Update(nukiAccount);
		
		// Assert
		nukiAccount.Id = 1;
		res.Value.Should().BeEquivalentTo(nukiAccount);
	}

	#endregion


}
