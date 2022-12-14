using FluentAssertions;
using FluentResults;
using Flurl.Http.Testing;
using Kerbero.Data.Common.Helpers;
using Kerbero.Data.SmartLocks.Repositories;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Errors;
using Kerbero.Domain.SmartLocks.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Data.Tests.SmartLocks.Repositories;

public class NukiSmartLockRepositoryTests
{
  private readonly Mock<ILogger<NukiSmartLockRepository>> _loggerMock = new();
  private readonly Mock<ILogger<NukiRestApiClient>> _httpLoggerMock = new();

  private readonly IConfiguration _configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string>
    {
      { "NUKI_DOMAIN", "http://fake.domain" }
    }!)
    .Build();

  private readonly NukiRestApiClient _nukiRestApiClient;

  public NukiSmartLockRepositoryTests()
  {
    _nukiRestApiClient = new NukiRestApiClient(_httpLoggerMock.Object, _configuration);
  }

  [Fact]
  public async Task GetById_Returns_A_Smartlock()
  {
    // Arrange
    var httpTest = new HttpTest();
    var rawResponse = await File.ReadAllTextAsync("JsonData/get-nuki-smartlock-response.json");
    httpTest.RespondWith(rawResponse);
    var repository = new NukiSmartLockRepository(_nukiRestApiClient);

    var credentials = new NukiCredentialModel()
    {
      Id = 1,
      Token = "VALID_TOKEN",
      NukiEmail = "test@nuki.com"
    };
    // Act
    var actual = await repository.Get(credentials, "ID");

    // Assert
    var expected = new SmartLockModel()
    {
      Id = "0",
      State = ESmartLockState.Unknown,
      Name = "string",
      SmartLockProvider = SmartLockProvider.Nuki
    };
    actual.IsSuccess.Should().BeTrue();
    actual.Should().BeEquivalentTo(Result.Ok(expected));
  }

  [Fact]
  public async Task GetById_Handles_NukiApiErrors()
  {
    // Arrange

    var repository = new NukiSmartLockRepository(_nukiRestApiClient);

    var credentials = new NukiCredentialModel()
    {
      Id = 1,
      Token = "A_TOKEN",
      NukiEmail = "test@nuki.com"
    };

    var httpTest = new HttpTest();

    #region Nuki 401 error

    httpTest.RespondWith("", 401);

    // Act
    var actual = await repository.Get(credentials, "ID");

    // Assert
    actual.IsFailed.Should().BeTrue();
    actual.Should().BeEquivalentTo(Result.Fail(new UnauthorizedAccessError()));

    #endregion

    #region Nuki 403 error

    httpTest.RespondWith("", 403);

    // Act
    actual = await repository.Get(credentials, "ID");

    // Assert
    actual.IsFailed.Should().BeTrue();
    actual.Should().BeEquivalentTo(Result.Fail(new UnauthorizedAccessError()));

    #endregion

    #region Nuki 404 error

    httpTest.RespondWith("", 404);

    // Act
    actual = await repository.Get(credentials, "ID");

    // Assert
    actual.IsFailed.Should().BeTrue();
    actual.Should().BeEquivalentTo(Result.Fail(new SmartLockNotFoundError("ID")));

    #endregion
  }
}