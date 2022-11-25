using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Errors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.NukiAuthentication.Entities;
using Kerbero.Infrastructure.NukiAuthentication.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Infrastructure.Tests.NukiAuthentication.Repositories;

public class NukiCredentialRepositoryTests: IDisposable
{
	private readonly NukiCredentialRepository _repository;
	private readonly ApplicationDbContext _dbContext;

	public NukiCredentialRepositoryTests()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: "AppDbContext")
			.Options;
		var logger = new Mock<ILogger<NukiCredentialRepository>>();
		_dbContext = new ApplicationDbContext(options);
		_repository = new NukiCredentialRepository(_dbContext, logger.Object);
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
		var nukiCredentials = new NukiCredential()
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			TokenExpiringTimeInSeconds = 2592000,
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
		};
		
		var res = await CreateHelper(nukiCredentials);
		
		// Assert
		nukiCredentials.Id = 1;
		res.Value.Should().BeEquivalentTo(nukiCredentials);
	}

	private async Task<Result<NukiCredential>> CreateHelper(NukiCredential nukiCredential)
	{
		_repository.Should().BeAssignableTo<INukiCredentialRepository>();

		// Act
		return await _repository.Create(nukiCredential);
	}
	#endregion

	#region GetNukiAccount

	[Fact]
	public async Task GetNukiAccount_Success_Test()
	{
		// Arrange
		for (var i = 1; i < 3; i++)
		{
			var nukiCredential = new NukiCredentialEntity()
			{
				Token = "VALID_TOKEN" + i,
				RefreshToken = "VALID_REFRESH_TOKEN",
				TokenExpiringTimeInSeconds = 2592000,
				ClientId = "VALID_CLIENT_ID" + i,
				TokenType = "bearer",
			};
			_dbContext.NukiCredentials.Add(nukiCredential);
		}

		await _dbContext.SaveChangesAsync();
		
		_repository.Should().BeAssignableTo<INukiCredentialRepository>();

		// Act
		var res = await _repository.GetById(1);
		
		// Assert
		res.Value.Should().BeEquivalentTo(new NukiCredentialEntity()
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
	public async Task GetNukiAccount_NotValidProviderAccountFoundError_Test()
	{
		// Arrange

		// Act
		var res = await _repository.GetById(0);
	
		// Assert
		res.IsFailed.Should().BeTrue();
		res.Errors.First().Should().BeEquivalentTo(new NukiAccountNotFoundError());
	}

	#endregion
	
	#region UpdateNukiAccount
	
	[Fact]
	public async Task Update_Success_Test()
	{
		// Arrange
		_repository.Should().BeAssignableTo<INukiCredentialRepository>();
		var nukiCredential = new NukiCredential()
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			TokenExpiringTimeInSeconds = 2592000,
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
		};
		await CreateHelper(nukiCredential);

		// Act
		var res = await _repository.Update(nukiCredential);
		
		// Assert
		nukiCredential.Id = 1;
		res.Value.Should().BeEquivalentTo(nukiCredential);
	}

	#endregion


}
