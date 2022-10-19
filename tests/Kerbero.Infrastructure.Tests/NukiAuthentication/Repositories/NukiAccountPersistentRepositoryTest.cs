using FluentAssertions;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.NukiAuthentication.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;

namespace Kerbero.Infrastructure.Tests.NukiAuthentication.Repositories;

public class NukiAccountPersistentRepositoryTest
{
	private readonly NukiAccountPersistentRepository _repository;
	private readonly Mock<IApplicationDbContext> _dbContext;
	private readonly Mock<DbSet<NukiAccount>> _dbSetNukiAccount;

	public NukiAccountPersistentRepositoryTest()
	{
		var logger = new Mock<ILogger<NukiAccountPersistentRepository>>();
		_dbSetNukiAccount = new Mock<DbSet<NukiAccount>>();
		_dbContext = new Mock<IApplicationDbContext>();
		_dbContext.Setup(c => c.NukiAccounts).Returns(_dbSetNukiAccount.Object);
		_repository = new NukiAccountPersistentRepository(_dbContext.Object, logger.Object);
	}

	#region CreateNukiAccount
	
	[Fact]
	public async Task CreateNukiAccount_Success_Test()
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
		
		var data = new NukiAccount
		{
			Id = 1,
			Token = nukiAccount.Token,
			RefreshToken = nukiAccount.RefreshToken,
			TokenExpiringTimeInSeconds = nukiAccount.TokenExpiringTimeInSeconds,
			ClientId = nukiAccount.ClientId,
			TokenType = nukiAccount.TokenType,
		};
		
		// Workaround to mock an EntityEntry, MS not provides a way to test a context with EF 6+
		var internalEntityEntry = new InternalEntityEntry(
			new Mock<IStateManager>().Object,
			new RuntimeEntityType("NukiAccount", typeof(NukiAccount), false, null!, null, null, ChangeTrackingStrategy.Snapshot, null, false),
			data);

		var entityEntry = new Mock<EntityEntry<NukiAccount>>(internalEntityEntry);
		_dbSetNukiAccount.Setup(c => c.Add(It.IsAny<NukiAccount>()))
			.Returns(entityEntry.Object);
		entityEntry.Setup(c => c.Entity).Returns(data);

		
		// Act
		var res = await _repository.Create(nukiAccount);
		
		// Assert
		nukiAccount.Id = 1;
		res.Value.Should().BeEquivalentTo(nukiAccount);
		res.Value.Should().NotBeSameAs(nukiAccount);
		_dbSetNukiAccount.Verify(c => c.Add(It.Is<NukiAccount>(e => e.Equals(nukiAccount))));
		_dbContext.Verify(c => c.SaveChangesAsync(CancellationToken.None));
	}

	[Fact]
	public async Task CreateNukiAccount_PersistentResourceNotAvailable_Test()
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

		_dbSetNukiAccount.Setup(c => c.Add(It.IsAny<NukiAccount>()))
			.Throws(new NotSupportedException());

		// Act
		var res = await _repository.Create(nukiAccount);

		// Assert
		res.IsFailed.Should().BeTrue();
		res.Errors.First().Should().BeEquivalentTo(new PersistentResourceNotAvailableError());
	}
	
	[Fact]
	public async Task CreateNukiAccount_DuplicateEntry_Test()
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

		int.TryParse(PostgresErrorCodes.UniqueViolation, out var errCode);
		_dbSetNukiAccount.Setup(c => c.Add(It.IsAny<NukiAccount>()))
			.Throws(new DbUpdateException("something", new DbUpdateException("something", new NpgsqlException{HResult = errCode})));

		// Act
		var res = await _repository.Create(nukiAccount);

		// Assert
		res.IsFailed.Should().BeTrue();
		res.Errors.First().Should().BeEquivalentTo(new DuplicateEntryError("Nuki account"));
		res.Errors.First().Message.Should().Contain("The Nuki account already exists, try to update instead.");
	}
	
	#endregion

	#region GetNukiAccount

	[Fact]
	public void GetNukiAccount_Success_Test()
	{
		// Arrange
		var dataList = new List<NukiAccount>();
		for (var i = 0; i < 3; i++)
		{
			var nukiAccount = new NukiAccount
			{
				Id = i,
				Token = "VALID_TOKEN",
				RefreshToken = "VALID_REFRESH_TOKEN",
				TokenExpiringTimeInSeconds = 2592000,
				ClientId = "VALID_CLIENT_ID",
				TokenType = "bearer",
			};
			dataList.Add(nukiAccount);
		}
		
		var data = dataList.AsQueryable();

		var mockSet = new Mock<DbSet<NukiAccount>>();
		mockSet.As<IQueryable<NukiAccount>>().Setup(m => m.Provider).Returns(data.Provider);
		mockSet.As<IQueryable<NukiAccount>>().Setup(m => m.Expression).Returns(data.Expression);
		mockSet.As<IQueryable<NukiAccount>>().Setup(m => m.ElementType).Returns(data.ElementType);
		mockSet.As<IQueryable<NukiAccount>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
		
		_repository.Should().BeAssignableTo<INukiAccountPersistentRepository>();

		_dbContext.Setup(c => c.NukiAccounts).Returns(mockSet.Object);
		
		// Act
		var res = _repository.GetAccount(1);
		
		// Assert
		res.Value.Should().BeEquivalentTo(new NukiAccount
		{
			Id = 1,
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			TokenExpiringTimeInSeconds = 2592000,
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
		});
	}

	[Fact]
	public void GetNukiAccount_NotValidProviderAccountFoundError_Test()
	{
		// Arrange
		var dataList = new List<NukiAccount>();
		for (var i = 2; i < 5; i++)
		{
			var nukiAccount = new NukiAccount
			{
				Id = i,
				Token = "VALID_TOKEN",
				RefreshToken = "VALID_REFRESH_TOKEN",
				TokenExpiringTimeInSeconds = 2592000,
				ClientId = "VALID_CLIENT_ID",
				TokenType = "bearer",
			};
			dataList.Add(nukiAccount);
		}
		
		var data = dataList.AsQueryable();

		var mockSet = new Mock<DbSet<NukiAccount>>();
		mockSet.As<IQueryable<NukiAccount>>().Setup(m => m.Provider).Returns(data.Provider);
		mockSet.As<IQueryable<NukiAccount>>().Setup(m => m.Expression).Returns(data.Expression);
		mockSet.As<IQueryable<NukiAccount>>().Setup(m => m.ElementType).Returns(data.ElementType);
		mockSet.As<IQueryable<NukiAccount>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
		
		_repository.Should().BeAssignableTo<INukiAccountPersistentRepository>();

		_dbContext.Setup(c => c.NukiAccounts).Returns(mockSet.Object);
		
		// Act
		var res = _repository.GetAccount(1);
	
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
		
		var data = new NukiAccount
		{
			Id = 1,
			Token = nukiAccount.Token,
			RefreshToken = nukiAccount.RefreshToken,
			TokenExpiringTimeInSeconds = nukiAccount.TokenExpiringTimeInSeconds,
			ClientId = nukiAccount.ClientId,
			TokenType = nukiAccount.TokenType,
		};
		
		// Workaround to mock an EntityEntry, MS not provides a way to test a context with EF 6+
		var internalEntityEntry = new InternalEntityEntry(
			new Mock<IStateManager>().Object,
			new RuntimeEntityType("NukiAccount", typeof(NukiAccount), false, null!, null, null, ChangeTrackingStrategy.Snapshot, null, false),
			data);

		var entityEntry = new Mock<EntityEntry<NukiAccount>>(internalEntityEntry);
		_dbSetNukiAccount.Setup(c => c.Update(It.IsAny<NukiAccount>()))
			.Returns(entityEntry.Object);
		entityEntry.Setup(c => c.Entity).Returns(data);

		
		// Act
		var res = await _repository.Update(nukiAccount);
		
		// Assert
		nukiAccount.Id = 1;
		res.Value.Should().BeEquivalentTo(nukiAccount);
		res.Value.Should().NotBeSameAs(nukiAccount);
		_dbSetNukiAccount.Verify(c => c.Update(It.Is<NukiAccount>(e => e.Equals(nukiAccount))));
		_dbContext.Verify(c => c.SaveChangesAsync(CancellationToken.None));
	}

	[Fact]
	public async Task Update_PersistentResourceNotAvailable_Test()
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

		_dbSetNukiAccount.Setup(c => c.Update(It.IsAny<NukiAccount>()))
			.Throws(new NotSupportedException());

		// Act
		var res = await _repository.Update(nukiAccount);

		// Assert
		res.IsFailed.Should().BeTrue();
		res.Errors.First().Should().BeEquivalentTo(new PersistentResourceNotAvailableError());
	}
	
	[Fact]
	public async Task UpdateNukiAccount_DuplicateEntry_Test()
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

		int.TryParse(PostgresErrorCodes.UniqueViolation, out var errCode);
		_dbSetNukiAccount.Setup(c => c.Update(It.IsAny<NukiAccount>()))
			.Throws(new DbUpdateException("something", new DbUpdateException("something", new NpgsqlException{HResult = errCode})));

		// Act
		var res = await _repository.Update(nukiAccount);

		// Assert
		res.IsFailed.Should().BeTrue();
		res.Errors.First().Should().BeEquivalentTo(new DuplicateEntryError("Nuki account"));
		res.Errors.First().Message.Should().Contain("The Nuki account already exists, try to update instead.");
	}
	
	#endregion
}
