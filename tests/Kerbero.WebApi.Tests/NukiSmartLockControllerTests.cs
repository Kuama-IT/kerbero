using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.Common.Models;
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
	private readonly Mock<IGetNukiSmartLocksInteractor> _interactor;
	private readonly Mock<IAuthenticateNukiAccountInteractor> _authInteractor;

	public NukiSmartLockControllerTests()
	{
		_authInteractor = new Mock<IAuthenticateNukiAccountInteractor>();
		_interactor = new Mock<IGetNukiSmartLocksInteractor>();
		_controller = new NukiSmartLockController(_interactor.Object, _authInteractor.Object);
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
		_interactor.Setup(c => c.Handle(It.IsAny<NukiSmartLocksPresentationRequest>()))
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
	
}
