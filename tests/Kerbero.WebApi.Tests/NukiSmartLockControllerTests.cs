using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiActions.Interactors;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kerbero.WebApi.Tests;

public class NukiSmartLockControllerTests
{
	private readonly NukiSmartLockController _controller;
	private readonly Mock<IAuthenticateNukiAccountInteractor> _authInteractor;
	private readonly Mock<ICreateNukiSmartLockInteractor> _createNukiSmartLockInteractor;
	private readonly Mock<IGetNukiSmartLocksInteractor> _getNukiSmartLockListInteractor;

	public NukiSmartLockControllerTests()
	{
		_getNukiSmartLockListInteractor = new Mock<IGetNukiSmartLocksInteractor>();
		_createNukiSmartLockInteractor = new Mock<ICreateNukiSmartLockInteractor>();
		_authInteractor = new Mock<IAuthenticateNukiAccountInteractor>();
		_controller = new NukiSmartLockController(_authInteractor.Object, _getNukiSmartLockListInteractor.Object, _createNukiSmartLockInteractor.Object);
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
		_getNukiSmartLockListInteractor.Setup(c => c.Handle(It.IsAny<NukiSmartLocksPresentationRequest>()))
			.Returns(() => Task.FromResult(Result.Ok(new List<KerberoSmartLockPresentationResponse>
			{
				new()
				{
					ExternalName = "kquarter",
					ExternalType = 2,
					ExternalAccountId = 1,
					ExternalSmartLockId = 1
				}
			})));
		
		// Act
		var resp = await _controller.GetSmartLocksByKerberoAccount(1);
		
		// Assert
		resp.Should().BeOfType<OkObjectResult>();
		var okResult = resp as OkObjectResult;
		okResult?.Value.Should().BeEquivalentTo(new List<KerberoSmartLockPresentationResponse>
		{
			new()
			{
				ExternalName = "kquarter",
				ExternalType = 2,
				ExternalAccountId = 1,
				ExternalSmartLockId = 1
			}
		});
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
				ExternalAccountId = 1,
				ExternalSmartLockId = 1
			})));

		// Act
		var resp = await _controller.CreateNukiSmartLockById(1, 1);
		var okResult = resp as OkObjectResult;

		// Assert
		okResult?.Value.Should().BeEquivalentTo(new KerberoSmartLockPresentationResponse
		{
			ExternalName = "kquarter",
			ExternalType = 2,
			ExternalAccountId = 1,
			ExternalSmartLockId = 1
		});
		_createNukiSmartLockInteractor.Verify(c => c.Handle(
			It.Is<CreateNukiSmartLockPresentationRequest>(p => p.NukiSmartLockId == 1)));
	}
	
}
