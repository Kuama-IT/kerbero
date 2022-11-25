using System.Net;
using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Kerbero.WebApi.Tests.Controllers;

public class NukiCredentialsControllerTests
{
  private readonly NukiCredentialsController _controller;

  private readonly Mock<ICreateNukiCredentialInteractor> _createNukiCredentialInteractorMock =
    new Mock<ICreateNukiCredentialInteractor>();

  private readonly Mock<ICreateNukiCredentialDraftInteractor> _createNukiCredentialDraftInteractorMock =
    new Mock<ICreateNukiCredentialDraftInteractor>();

  private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();

  public NukiCredentialsControllerTests()
  {
    _controller = new NukiCredentialsController(
      _createNukiCredentialDraftInteractorMock.Object,
      _createNukiCredentialInteractorMock.Object,
      _configurationMock.Object
    );
  }

  [Fact]
  public async Task RetrieveToken_Success_Test()
  {
    // Arrange
    var credentials = new NukiCredentialDto
    {
      Id = 1,
      ClientId = "VALID_CLIENT_ID"
    };
    
    _createNukiCredentialInteractorMock
      .Setup(c => c.Handle(It.IsAny<CreateNukiCredentialParams>()))
      .Returns(async () => await Task.FromResult(Result.Ok(credentials)));

    // Act
    var result = await _controller.ConfirmDraft("VALID_CLIENT_ID", "VALID_CODE");
    var response = result.Result as ObjectResult;
    
    // Assert
    _createNukiCredentialInteractorMock.Verify(c =>
      c.Handle(It.Is<CreateNukiCredentialParams>(p =>
        p.Code.Equals("VALID_CODE") && p.ClientId.Equals("VALID_CLIENT_ID"))));
    
    response?.Value.Should().BeOfType<NukiCredentialDto>();
    response?.Value.Should().BeEquivalentTo(credentials);
  }


  public static IEnumerable<object[]> ErrorToTest =>
    new List<object[]>
    {
      new object[] { new ExternalServiceUnreachableError() },
      new object[] { new UnableToParseResponseError() },
      new object[] { new UnauthorizedAccessError() },
      new object[] { new InvalidParametersError("VALID_CLIENT_ID") },
      new object[] { new DuplicateEntryError("Nuki account") },
      new object[] { new UnknownExternalError() },
      new object[] { new PersistentResourceNotAvailableError() }
    };

  [MemberData(nameof(ErrorToTest))]
  [Theory]
  public async Task RetrieveToken_KerberoError_Test(KerberoError error)
  {
    // Arrange
    _createNukiCredentialInteractorMock.Setup(c => c.Handle(It.IsAny<CreateNukiCredentialParams>()))
      .Returns(async () => await Task.FromResult(Result.Fail(error)));

    // Act

    // Assert
    var action =
      (await _controller.ConfirmDraft("VALID_CLIENT_ID", "VALID_CODE")).Result as
      ObjectResult;
    action?.Value.Should().NotBeNull().And.BeEquivalentTo(error);
    switch (error)
    {
      case InvalidParametersError:
      case DuplicateEntryError:
        action?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        break;
      case UnknownExternalError:
        action?.StatusCode.Should().Be((int)HttpStatusCode.ServiceUnavailable);
        break;
      case UnableToParseResponseError:
      case ExternalServiceUnreachableError:
      case PersistentResourceNotAvailableError:
        action?.StatusCode.Should().Be((int)HttpStatusCode.BadGateway);
        break;
      case UnauthorizedAccessError:
        action?.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        break;
      case not null:
        action?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        break;
    }
  }
}