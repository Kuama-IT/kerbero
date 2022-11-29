using FluentAssertions;
using FluentResults;
using Kerbero.Domain.NukiAuthentication.Errors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.NukiAuthentication.Entities;
using Kerbero.Infrastructure.NukiAuthentication.Mappers;
using Kerbero.Infrastructure.NukiAuthentication.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Infrastructure.Tests.NukiAuthentication.Repositories;

public class NukiCredentialRepositoryTests
{
  private readonly Mock<ILogger<NukiCredentialRepository>> _loggerMock = new();

  [Fact]
  public async Task Create_ValidInput_ReturnCorrectResult()
  {
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: "NukiCredentials_Create")
      .Options;
    await using var dbContext = new ApplicationDbContext(options);

    var repository = new NukiCredentialRepository(dbContext, _loggerMock.Object);

    var tNukiCredential = new NukiCredentialModel()
    {
      Token = "VALID_TOKEN",
    };

    // Act
    var actual = await repository.Create(tNukiCredential, new Guid());

    var expected = new NukiCredentialModel()
    {
      Id = 1,
      Token = "VALID_TOKEN",
    };

    // Assert
    actual.IsSuccess.Should().BeTrue();
    actual.Should().BeEquivalentTo(Result.Ok(expected));
  }


  [Fact]
  public async Task GetById_ValidInput_ReturnCorrectResult()
  {
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: "NukiCredentials_GetById")
      .Options;
    await using var dbContext = new ApplicationDbContext(options);
    var repository = new NukiCredentialRepository(dbContext, _loggerMock.Object);

    var tNukiCredentialTable = new NukiCredentialEntity()
    {
      Token = "VALID_TOKEN",
    };

    dbContext.NukiCredentials.Add(tNukiCredentialTable);
    await dbContext.SaveChangesAsync();


    // Act
    var actual = await repository.GetById(1);

    // Assert
    var expected = Result.Ok(new NukiCredentialModel()
    {
      Id = 1,
      Token = "VALID_TOKEN",
    });

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task GetById_InvalidId_ReturnNukiCredentialNotFound()
  {
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: "NukiCredentials_GetById_InvalidId")
      .Options;
    await using var dbContext = new ApplicationDbContext(options);
    var repository = new NukiCredentialRepository(dbContext, _loggerMock.Object);

    // Act
    var actual = await repository.GetById(1);

    // Assert
    var expected = Result.Fail(new NukiCredentialNotFoundError());

    actual.IsFailed.Should().BeTrue();
    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task Update_ValidInput_ReturnCorrectResult()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: "NukiCredentials_Update")
      .Options;
    await using var dbContext = new ApplicationDbContext(options);
    var repository = new NukiCredentialRepository(dbContext, _loggerMock.Object);

    // Arrange
    var tNukiCredentialTable = new NukiCredentialEntity()
    {
      Token = "VALID_TOKEN",
    };

    dbContext.NukiCredentials.Add(tNukiCredentialTable);
    await dbContext.SaveChangesAsync();

    var tNukiCredential = NukiCredentialMapper.Map(tNukiCredentialTable);

    // Act
    var actual = await repository.Update(tNukiCredential);

    // Assert
    actual.Should().BeEquivalentTo(Result.Ok(tNukiCredential));
  }
}
