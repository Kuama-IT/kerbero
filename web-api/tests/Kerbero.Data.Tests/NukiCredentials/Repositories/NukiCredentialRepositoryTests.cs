using FluentAssertions;
using FluentResults;
using Kerbero.Data.Common.Context;
using Kerbero.Data.Common.Helpers;
using Kerbero.Data.NukiCredentials.Entities;
using Kerbero.Data.NukiCredentials.Repositories;
using Kerbero.Domain.NukiCredentials.Errors;
using Kerbero.Domain.NukiCredentials.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Data.Tests.NukiCredentials.Repositories;

public class NukiCredentialRepositoryTests
{
  private readonly Mock<ILogger<NukiCredentialRepository>> _loggerMock = new();
  private readonly Mock<ILogger<NukiRestApiClient>> _httpLoggerMock = new();

  private readonly IConfiguration _configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string>
    {
      { "NUKI_DOMAIN", "http://fake.domain" }
    }!)
    .Build();

  private readonly NukiRestApiClient _nukiRestApiClient;

  public NukiCredentialRepositoryTests()
  {
    _nukiRestApiClient = new NukiRestApiClient(_httpLoggerMock.Object, _configuration);
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
      _nukiRestApiClient
    );

    var tNukiCredential = new NukiCredentialModel()
    {
      Token = "VALID_TOKEN",
      NukiEmail = "test@nuki.com"
    };

    // Act
    var actual = await repository.Create(tNukiCredential, new Guid());

    var expected = new NukiCredentialModel()
    {
      Id = 1,
      Token = "VALID_TOKEN",
      NukiEmail = "test@nuki.com"
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
      _nukiRestApiClient
    );

    var tNukiCredentialTable = new NukiCredentialEntity()
    {
      Token = "VALID_TOKEN",
      NukiEmail = "test@nuki.com"
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
      NukiEmail = "test@nuki.com"
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
      _nukiRestApiClient
    );

    // Act
    var actual = await repository.GetById(1);

    // Assert
    var expected = Result.Fail(new NukiCredentialNotFoundError());

    actual.IsFailed.Should().BeTrue();
    actual.Should().BeEquivalentTo(expected);
  }
}