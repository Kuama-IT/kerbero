using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiActions.Interactors;
using Kerbero.Domain.NukiActions.Errors;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.WebApi.Controllers;
using Kerbero.WebApi.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kerbero.WebApi.Tests;

public class NukiSmartLockControllerTests
{
	private readonly NukiSmartLockController _controller;
	private readonly Mock<IGetNukiSmartLocksInteractor> _getNukiSmartLocksInteractor;
	private readonly Mock<IAuthenticateNukiAccountInteractor> _authInteractor;
	private readonly Mock<ICreateNukiSmartLockInteractor> _createNukiSmartLockInteractor;
	private readonly Mock<IOpenNukiSmartLockInteractor> _openNukiSmartLockInteractor;
	private readonly Mock<ICloseNukiSmartLockInteractor> _closeNukiSmartLockInteractor;

	public NukiSmartLockControllerTests()
	{
		_getNukiSmartLocksInteractor = new Mock<IGetNukiSmartLocksInteractor>();
		_createNukiSmartLockInteractor = new Mock<ICreateNukiSmartLockInteractor>();
		_authInteractor = new Mock<IAuthenticateNukiAccountInteractor>();
		_openNukiSmartLockInteractor = new Mock<IOpenNukiSmartLockInteractor>();
		_closeNukiSmartLockInteractor = new Mock<ICloseNukiSmartLockInteractor>();
		_controller = new NukiSmartLockController(_authInteractor.Object, 
			_getNukiSmartLocksInteractor.Object, 
			_createNukiSmartLockInteractor.Object,
			_openNukiSmartLockInteractor.Object,
			_closeNukiSmartLockInteractor.Object);
	}

	[Fact]
	public async Task GetSmartLocksListByKerberoAccount_Success()
	{
		// Arrange
		_authInteractor.Setup(a => a.Handle(It.IsAny<AuthenticateRepositoryPresentationRequest>()))
			.Returns(Task.FromResult(Result.Ok(
				new AuthenticateRepositoryPresentationResponse()
				{
					Token = "ACCESS_TOKEN"
				})));
		_getNukiSmartLocksInteractor.Setup(c => c.Handle(It.IsAny<NukiSmartLocksPresentationRequest>()))
			.Returns(() => Task.FromResult(Result.Ok(new List<KerberoSmartLockPresentationResponse>
			{
				new()
				{
					ExternalName = "kquarter",
					ExternalType = 2,
					AccountId = 1,
					ExternalSmartLockId = 1,
					SmartLockId = 1
				}
			})));
		
		// Act
		var resp = await _controller.GetSmartLocksByKerberoAccount(1);
		
		// Assert
		resp.Should().BeOfType<OkObjectResult>();
		var okResult = resp as OkObjectResult;
		okResult!.Value.Should().BeEquivalentTo(new List<KerberoSmartLockPresentationResponse>
		{
			new()
			{
				ExternalName = "kquarter",
				ExternalType = 2,
				AccountId = 1,
				ExternalSmartLockId = 1,
				SmartLockId = 1
			}
		});
	}

	[Fact]
	public async Task CloseNukiSmartLockById_Success_Test()
	{
		// Arrange
		_authInteractor.Setup(i => i.Handle(It.IsAny<AuthenticateRepositoryPresentationRequest>()))
			.Returns(Task.FromResult(Result.Ok(new AuthenticateRepositoryPresentationResponse
			{
				Token = "ACCESS_TOKEN"
			})));
		_closeNukiSmartLockInteractor.Setup(i => i.Handle(It.IsAny<CloseNukiSmartLockPresentationRequest>()))
			.Returns(Task.FromResult(Result.Ok()));
		
		// Act
		var res = await _controller.CloseSmartLockById(new CloseNukiSmartLockRequest(1, 0));

		// Assert
		_closeNukiSmartLockInteractor.Verify(i =>
			i.Handle(It.Is<CloseNukiSmartLockPresentationRequest>(x => x.AccessToken == "ACCESS_TOKEN")));
		res.Should().BeAssignableTo<OkResult>();
	}
	
	[Fact]
	public async Task CreateNukiSmartLockByIdAndKerberoAccount_Success_Test()
	{
		// Arrange
		_authInteractor.Setup(a => a.Handle(It.IsAny<AuthenticateRepositoryPresentationRequest>()))
			.Returns(Task.FromResult(Result.Ok(
				new AuthenticateRepositoryPresentationResponse()
				{
					Token = "ACCESS_TOKEN"
				})));
		_createNukiSmartLockInteractor.Setup(c => c.Handle(It.IsAny<CreateNukiSmartLockPresentationRequest>()))
			.Returns(Task.FromResult(Result.Ok(new KerberoSmartLockPresentationResponse
			{
				ExternalName = "kquarter",
				ExternalType = 2,
				AccountId = 1,
				ExternalSmartLockId = 1,
				SmartLockId = 1
			})));

		// Act
		var resp = await _controller.CreateNukiSmartLockById(new CreateNukiSmartLockRequest(1, 1));
		var okResult = resp as OkObjectResult;

		// Assert
		okResult!.Value.Should().BeEquivalentTo(new KerberoSmartLockPresentationResponse
		{
			ExternalName = "kquarter",
			ExternalType = 2,
			AccountId = 1,
			ExternalSmartLockId = 1,
			SmartLockId = 1
		});
		_createNukiSmartLockInteractor.Verify(c => c.Handle(
			It.Is<CreateNukiSmartLockPresentationRequest>(p => p.NukiSmartLockId == 1)));
	}
	
	[Fact]
	public async Task OpenNukiSmartLockByIdAndKerberoAccount_Success_Test()
	{
		// Arrange
		_authInteractor.Setup(i => i.Handle(It.IsAny<AuthenticateRepositoryPresentationRequest>()))
			.Returns(Task.FromResult(Result.Ok(new AuthenticateRepositoryPresentationResponse
			{
				Token = "ACCESS_TOKEN"
			})));
		_openNukiSmartLockInteractor.Setup(c => c.Handle(It.IsAny<OpenNukiSmartLockPresentationRequest>()))
			.Returns(Task.FromResult(Result.Ok()));
		
		// Act
		var result = await _controller.OpenNukiSmartLockById(new OpenNukiSmartLockRequest(1, 1)) as OkResult;

		// Assert
		_openNukiSmartLockInteractor.Verify(i =>
			i.Handle(It.Is<OpenNukiSmartLockPresentationRequest>(x =>
				x.AccessToken == "ACCESS_TOKEN" && x.NukiSmartLockId == 1)));
		result!.StatusCode.Should().Be(200);
	}
	
	[Fact]
	public async Task OpenNukiSmartLockByIdAndKerberoAccount_SmartLockNotFound_Test()
	{
		// Arrange
		_authInteractor.Setup(i => i.Handle(It.IsAny<AuthenticateRepositoryPresentationRequest>()))
			.Returns(Task.FromResult(Result.Ok(new AuthenticateRepositoryPresentationResponse
			{
				Token = "ACCESS_TOKEN"
			})));
		_openNukiSmartLockInteractor.Setup(c => c.Handle(It.IsAny<OpenNukiSmartLockPresentationRequest>()))
			.Returns(Task.FromResult(Result.Fail(new SmartLockNotFoundError())));
		
		// Act
		var result = await _controller.OpenNukiSmartLockById(new OpenNukiSmartLockRequest(1, 1)) as ObjectResult;

		// Assert
		result!.StatusCode.Should().Be(400);
	}
	
	[Fact]
	public async Task OpenNukiSmartLockByIdAndKerberoAccount_SmartLockNotReachable_Test()
	{
		// Arrange
		_authInteractor.Setup(i => i.Handle(It.IsAny<AuthenticateRepositoryPresentationRequest>()))
			.Returns(Task.FromResult(Result.Ok(new AuthenticateRepositoryPresentationResponse
			{
				Token = "ACCESS_TOKEN"
			})));
		_openNukiSmartLockInteractor.Setup(c => c.Handle(It.IsAny<OpenNukiSmartLockPresentationRequest>()))
			.Returns(Task.FromResult(Result.Fail(new SmartLockNotReachableError())));

		// Act
		var result = await _controller.OpenNukiSmartLockById(new OpenNukiSmartLockRequest(1, 1)) as ObjectResult;

		// Assert
		result!.StatusCode.Should().Be(502);
	}
	[Fact]
	public async Task CloseNukiSmartLockById_SmartLockNotFound_Test()
	{
		// Arrange
		_authInteractor.Setup(i => i.Handle(It.IsAny<AuthenticateRepositoryPresentationRequest>()))
			.Returns(Task.FromResult(Result.Ok(new AuthenticateRepositoryPresentationResponse
			{
				Token = "ACCESS_TOKEN"
			})));
		_closeNukiSmartLockInteractor.Setup(i => i.Handle(It.IsAny<CloseNukiSmartLockPresentationRequest>()))
			.Returns(Task.FromResult(Result.Fail(new SmartLockNotFoundError())));
		
		// Act
		var res = await _controller.CloseSmartLockById(new CloseNukiSmartLockRequest(1, 0)) as ObjectResult;

		// Assert
		res!.Value.Should().BeEquivalentTo(new SmartLockNotFoundError());
	}
}
