using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiActions.Interactors;
using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;
using Kerbero.Domain.NukiActions.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiActions.Interactors;

public class GetNukiSmartLocksListInteractorTest
{
	private readonly GetNukiSmartLocksInteractor _interactor;
	private readonly Mock<INukiSmartLockExternalRepository> _nukiClient;

	public GetNukiSmartLocksListInteractorTest()
	{
		_nukiClient = new Mock<INukiSmartLockExternalRepository>();
		_interactor = new GetNukiSmartLocksInteractor(_nukiClient.Object);
	}

	[Fact]
	public async Task GetNukiSmartLocksList_Success_Test()
	{
		// Arrange
		var clientResponse = Task.FromResult(Result.Ok(new List<NukiSmartLockExternalResponse>()
		{
				new()
				{
					SmartLockId = 0,
					AccountId = 0,
					Type = 0,
					LmType = 0,
					AuthId = 0,
					Name = "kquarter",
					Favourite = true,
					State = new NukiSmartLockStateExternalResponse()
					{
						Mode = 4,
						State = 255,
						LastAction = 5,
						BatteryCritical = true,
						BatteryCharging = true,
						BatteryCharge = 100,
						DoorState = 255,
						OperationId = "string"
					}
				}
		}));
		_nukiClient.Setup(c => c.GetNukiSmartLocks(It.IsAny<string>()))
			.Returns(clientResponse);
		// Act
		var nukiSmartLocksList = await _interactor.Handle(new NukiSmartLocksPresentationRequest("ACCESS_TOKEN"));

		// Assert
		nukiSmartLocksList.IsSuccess.Should().BeTrue();
		nukiSmartLocksList.Value.Should().BeEquivalentTo(new List<KerberoSmartLockPresentationResponse>()
		{
			new()
				{
					ExternalSmartLockId = 0,
					ExternalAccountId = 0,
					ExternalType = 0,
					ExternalName = "kquarter"
				}
		});
	}	
	
	[Fact]
	public async Task GetNukiSmartLocksList_UnauthorizedRequest_Test()
	{
		// Arrange
		_nukiClient.Setup(c => c.GetNukiSmartLocks(It.IsAny<string>()))
			.Returns(async () => await Task.FromResult(Result.Fail(new UnauthorizedAccessError())));
		// Act
		var nukiSmartLocksList = await _interactor.Handle(new NukiSmartLocksPresentationRequest("ACCESS_TOKEN"));

		// Assert
		nukiSmartLocksList.IsFailed.Should().BeTrue();
		nukiSmartLocksList.Errors.First().Should().BeEquivalentTo(new UnauthorizedAccessError());
	}
}