using System.Net;
using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kerbero.WebApi.Tests;

public class NukiAuthenticationControllerTest
{
	private readonly NukiAuthenticationController _controller;
	private readonly Mock<ICreateNukiAccountInteractor> _interactorToken;
	private readonly Mock<IProvideNukiAuthRedirectUrlInteractor> _interactorCode;

	public NukiAuthenticationControllerTest()
	{
		_interactorToken = new Mock<ICreateNukiAccountInteractor>();
		_interactorCode = new Mock<IProvideNukiAuthRedirectUrlInteractor>();
		_controller = new NukiAuthenticationController(_interactorCode.Object, _interactorToken.Object);
	}
	
	[Fact]
	public void RedirectForCode_AndRedirect_Test()
	{
		// Arrange
		_interactorCode.Setup(c => c.Handle(It.IsAny<NukiRedirectPresentationRequest>()))
			.Returns(Result.Ok(new NukiRedirectPresentationResponse(new Uri("http://api.nuki.io/oauth/authorize?response_type=code" +
			                           "&client_id=v7kn_NX7vQ7VjQdXFGK43g" +
			                           "&redirect_uri=https://test.com/nuki/code/v7kn_NX7vQ7VjQdXFGK43g" + 
			                           "&scope=account notification smartlock smartlock.readOnly smartlock.action" +
			                           " smartlock.auth smartlock.config smartlock.log"))));
		
		// Act
		var redirect = _controller.RedirectByClientId("VALID_CLIENT_ID");

		// Assert
		_interactorCode.Verify(c => c.Handle(It.Is<NukiRedirectPresentationRequest>(s => s.ClientId.Contains("VALID_CLIENT_ID"))));
		redirect.Should().BeOfType<RedirectResult>();
	}
	
	[Fact]
	public Task RedirectForCode_ReturnInvalidParameters_Test()
	{
		// Arrange
		_interactorCode.Setup(c => c.Handle(It.IsAny<NukiRedirectPresentationRequest>()))
			.Returns(Result.Fail(new InvalidParametersError("client_id")));
		
		// Act
	
		// Assert
		var ex = _controller.RedirectByClientId("VALID_CLIENT_ID") as ObjectResult;
		ex.Should().NotBeNull();
		ex?.StatusCode.Should().Be(400);
		ex?.Value.Should().BeOfType<InvalidParametersError>();
		return Task.CompletedTask;
	}
	
	[Fact]
	public async Task RetrieveToken_Success_Test()
	{
		// Arrange
		var shouldResponseDto = new NukiAccountPresentationResponse
		{
			Id = 1,
			ClientId = "VALID_CLIENT_ID"
		};
		_interactorToken.Setup(c => c.Handle(It.IsAny<NukiAccountPresentationRequest>()))
			.Returns(async () => await Task.FromResult(Result.Ok(shouldResponseDto)));

		// Act
		var result = await _controller.RetrieveTokenByCode("VALID_CLIENT_ID", "VALID_CODE");
		var response = result.Result as ObjectResult;
		// Assert
		_interactorToken.Verify(c => 
			c.Handle(It.Is<NukiAccountPresentationRequest>( p => p.Code!.Equals("VALID_CODE") && p.ClientId.Equals("VALID_CLIENT_ID"))));
		response?.Value.Should().BeOfType<NukiAccountPresentationResponse>();
		response?.Value.Should().BeEquivalentTo(shouldResponseDto);
	}
	
	
	public static IEnumerable<object[]> ErrorToTest =>
		new List<object[]>
		{
			new object[] { new ExternalServiceUnreachableError()},
			new object[] { new UnableToParseResponseError()},
			new object[] { new UnauthorizedAccessError()},
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
		_interactorToken.Setup(c => c.Handle(It.IsAny<NukiAccountPresentationRequest>()))
			.Returns(async () => await Task.FromResult(Result.Fail(error)));
		
		// Act
	
		// Assert
		var action = (await _controller.RetrieveTokenByCode("VALID_CLIENT_ID", "VALID_CODE")).Result as ObjectResult;
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