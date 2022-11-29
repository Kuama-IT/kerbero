using FluentAssertions;
using FluentResults;
using Flurl.Http.Testing;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Errors;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Errors;
using Kerbero.Domain.SmartLocks.Models;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.NukiCredentials.Entities;
using Kerbero.Infrastructure.NukiCredentials.Repositories;
using Kerbero.Infrastructure.SmartLocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Infrastructure.Tests.SmartLocks.Repositories;

public class NukiSmartLockRepositoryTests
{
  private readonly Mock<ILogger<NukiSmartLockRepository>> _loggerMock = new();
  private readonly Mock<ILogger<NukiSafeHttpCallHelper>> _httpLoggerMock = new();

  private readonly IConfiguration _configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string>
    {
      { "NUKI_DOMAIN", "http://fake.domain" }
    }!)
    .Build();

  private readonly NukiSafeHttpCallHelper _nukiSafeHttpCallHelper;

  public NukiSmartLockRepositoryTests()
  {
    _nukiSafeHttpCallHelper = new NukiSafeHttpCallHelper(_httpLoggerMock.Object);
  }

  [Fact]
  public async Task GetById_Returns_A_Smartlock()
  {
    // Arrange
    var httpTest = new HttpTest();
    var rawResponse = await File.ReadAllTextAsync("JsonData/get-nuki-smartlock-response.json");
    httpTest.RespondWith(rawResponse);
    var repository = new NukiSmartLockRepository(_configuration, _nukiSafeHttpCallHelper);

    var credentials = new NukiCredentialModel()
    {
      Id = 1,
      Token = "VALID_TOKEN",
    };
    // Act
    var actual = await repository.Get(credentials, "ID");

    // Assert
    var expected = new SmartLock()
    {
      Id = "0",
      State = SmartLockState.Unknown,
      Name = "string",
      Provider = SmartlockProvider.Nuki
    };
    actual.IsSuccess.Should().BeTrue();
    actual.Should().BeEquivalentTo(Result.Ok(expected));
  }

  [Fact]
  public async Task GetById_Handles_NukiApiErrors()
  {
    // Arrange

    var repository = new NukiSmartLockRepository(_configuration, _nukiSafeHttpCallHelper);
    
    var credentials = new NukiCredentialModel()
    {
      Id = 1,
      Token = "A_TOKEN",
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