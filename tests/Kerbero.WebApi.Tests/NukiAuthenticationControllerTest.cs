using System.Net;
using System.Net.Http.Json;
using System.Web.Http;
using FluentAssertions;
using FluentResults;
using Kerbero.Common.Errors;
using Kerbero.Common.Errors.CreateNukiAccountErrors;
using Kerbero.Common.Interactors;
using Kerbero.Common.Interfaces;
using Kerbero.Common.Models;
using Kerbero.Common.Repositories;
using Kerbero.WebApi.Controllers;
using Kerbero.WebApi.Models;
using Kerbero.WebApi.Models.ErrorMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.WebApi.Tests;

public class NukiAuthenticationControllerTest
{
	private readonly NukiAuthenticationController _controller;
	private readonly Mock<InteractorAsync<NukiAccountExternalRequestDto, NukiAccountPresentationDto>> _interactorToken;
	private readonly Mock<Interactor<string, Uri>> _interactorCode;

	public NukiAuthenticationControllerTest()
	{
		_interactorToken = new Mock<InteractorAsync<NukiAccountExternalRequestDto, NukiAccountPresentationDto>>();
		_interactorCode = new Mock<Interactor<string, Uri>>();
		_controller = new NukiAuthenticationController(_interactorCode.Object, _interactorToken.Object);
	}
	
	[Fact]
	public void RedirectForCode_AndRedirect_Test()
	{
		// Arrange
		_interactorCode.Setup(c => c.Handle(It.IsAny<string>()))
			.Returns(Result.Ok(new Uri("http://api.nuki.io/oauth/authorize?response_type=code" +
			                           "&client_id=v7kn_NX7vQ7VjQdXFGK43g" +
			                           "&redirect_uri=https://test.com/nuki/code/v7kn_NX7vQ7VjQdXFGK43g" + 
			                           "&scope=account notification smartlock smartlock.readOnly smartlock.action" +
			                           " smartlock.auth smartlock.config smartlock.log")));
		
		// Act
		var redirect = _controller.RedirectForCode("VALID_CLIENT_ID");

		// Assert
		_interactorCode.Verify(c => c.Handle(It.Is<string>(s => s.Contains("VALID_CLIENT_ID"))));
		redirect.Should().BeOfType<RedirectResult>();
	}
	
	[Fact]
	public async Task RedirectForCode_ReturnInvalidParameters_Test()
	{
		// Arrange
		_interactorCode.Setup(c => c.Handle(It.IsAny<string>()))
			.Returns(Result.Fail(new InvalidParametersError("client_id")));
		
		// Act

		// Assert
		var ex = Assert.Throws<HttpResponseException>(() => _controller.RedirectForCode("VALID_CLIENT_ID"));
		ex.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var content = await ex.Response.Content.ReadFromJsonAsync<KerberoWebApiErrorResponse>();
		content.Should().NotBeNull().And.BeEquivalentTo( new KerberoWebApiErrorResponse()
		{
			Error = "InvalidParametersError",
			ErrorMessage = "There are missing or wrong parameter in the request: client_id."
		});
	}
	
	[Fact]
	public async Task RetrieveToken_Success_Test()
	{
		// Arrange
		var shouldResponseDto = new NukiAccountPresentationDto()
		{
			Id = 1,
			ClientId = "VALID_CLIENT_ID"
		};
		_interactorToken.Setup(c => c.Handle(It.IsAny<NukiAccountExternalRequestDto>()))
			.Returns(async () => await Task.FromResult(Result.Ok(shouldResponseDto)));

		// Act
		var response = await _controller.RetrieveToken("VALID_CLIENT_ID", "VALID_CODE");
		
		// Assert
		_interactorToken.Verify(c => 
			c.Handle(It.Is<NukiAccountExternalRequestDto>( p => p.Code.Equals("VALID_CODE") && p.ClientId.Equals("VALID_CLIENT_ID"))));
		response.Should().BeOfType<NukiAccountPresentationDto>();
		response.Should().BeEquivalentTo(shouldResponseDto);
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
	public async Task RedirectForCode_KerberoError_Test(KerberoError error)
	{
		// Arrange
		_interactorToken.Setup(c => c.Handle(It.IsAny<NukiAccountExternalRequestDto>()))
			.Returns(async () => await Task.FromResult(Result.Fail(error)));
		
		// Act

		// Assert
		var ex = await Assert.ThrowsAsync<HttpResponseException>(() => _controller.RetrieveToken("VALID_CLIENT_ID", "VALID_CODE"));
		var content = await ex.Response.Content.ReadFromJsonAsync<KerberoWebApiErrorResponse>();
		content.Should().NotBeNull().And.BeEquivalentTo( new KerberoWebApiErrorResponse()
		{
			Error = error.GetType().Name,
			ErrorMessage = error.Message
		});
		switch (error)
		{
			case InvalidParametersError:
			case DuplicateEntryError:
				ex.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
				break;
			case ExternalServiceUnreachableError:
			case PersistentResourceNotAvailableError:
			case UnknownExternalError:
				ex.Response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
				break;
			case UnableToParseResponseError:
				ex.Response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
				break;
			case UnauthorizedAccessError:
				ex.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
				break;
			case not null:
				ex.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
				break;
		}
	}
	
	
	[MemberData(nameof(ErrorToTest))]
	[Theory]
	public async Task Models_Map_Test(KerberoError error)
	{
		// Arrange
	
		// Act
		var ex = HttpResponseExceptionMap.Map(error);
		
		// Assert
		var content = await ex.Response.Content.ReadFromJsonAsync<KerberoWebApiErrorResponse>();
		content.Should().NotBeNull().And.BeEquivalentTo( new KerberoWebApiErrorResponse()
		{
			Error = error.GetType().Name,
			ErrorMessage = error.Message
		});
	}
}
