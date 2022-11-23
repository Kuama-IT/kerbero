using FluentAssertions;
using FluentResults;
using Kerbero.Domain.NukiCredentials.Errors;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.NukiCredentials.Entities;
using Kerbero.Infrastructure.NukiCredentials.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Infrastructure.Tests.NukiCredentials.Repositories;

public class NukiCredentialRepositoryTests
{
  private readonly Mock<ILogger<NukiCredentialRepository>> _loggerMock = new();
  private readonly Mock<ILogger<NukiSafeHttpCallHelper>> _httpLoggerMock = new();

  private readonly IConfiguration _configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string>
    {
      { "NUKI_DOMAIN", "http://fake.domain" }
    }!)
    .Build();

  private readonly NukiSafeHttpCallHelper _nukiSafeHttpCallHelper;

  public NukiCredentialRepositoryTests()
  {
    _nukiSafeHttpCallHelper = new NukiSafeHttpCallHelper(_httpLoggerMock.Object);
  }

  [Fact]
  public async Task Create_ValidInput_ReturnCorrectResult()
  {
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: "NukiCredentials_Create")
      .Options;
    await using var dbContext = new ApplicationDbContext(options);

    var repository = new NukiCredentialRepository(
      dbContext,
      _loggerMock.Object,
      _nukiSafeHttpCallHelper,
      _configuration
    );

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
    var repository = new NukiCredentialRepository(
      dbContext,
      _loggerMock.Object,
      _nukiSafeHttpCallHelper,
      _configuration
    );

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
    var repository = new NukiCredentialRepository(
      dbContext,
      _loggerMock.Object,
      _nukiSafeHttpCallHelper,
      _configuration
    );

    // Act
    var actual = await repository.GetById(1);

    // Assert
    var expected = Result.Fail(new NukiCredentialNotFoundError());

    actual.IsFailed.Should().BeTrue();
    actual.Should().BeEquivalentTo(expected);
  }
}