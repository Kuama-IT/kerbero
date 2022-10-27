using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Entities;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiActions.Interactors;
using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Moq;

namespace Kerbero.Common.Tests.NukiActions.Interactors;

public class GetNukiSmartLocksListInteractorTest
{
	private readonly GetNukiSmartLocksListInteractor _interactor;
	private readonly Mock<INukiSmartLockExternalRepository> _nukiClient;

	public GetNukiSmartLocksListInteractorTest()
	{
		_nukiClient = new Mock<INukiSmartLockExternalRepository>();
		_interactor = new GetNukiSmartLocksListInteractor(_nukiClient.Object);
	}

	[Fact]
	public async Task GetNukiSmartLocksList_Success_Test()
	{
		// Arrange
		var clientResponse = Task.FromResult(Result.Ok(new NukiSmartLocksListExternalResponseDto
		{
			NukiSmartLockList = new List<NukiSmartLockExternalResponseDto> {
				new()
				{
					SmartLockId = 0,
					AccountId = 0,
					Type = 0,
					LmType = 0,
					AuthId = 0,
					Name = "kquarter",
					Favourite = true,
					State = new NukiSmartLockState()
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
			}
		}));
		_nukiClient.Setup(c => c.GetNukiSmartLockList(It.IsAny<int>()))
			.Returns(clientResponse);
		// Act
		var nukiSmartLocksList = await _interactor.Handle(new NukiAuthenticatedRequestDto());

		// Assert
		nukiSmartLocksList.IsSuccess.Should().BeTrue();
		nukiSmartLocksList.Value.Should().BeEquivalentTo(new NukiSmartLocksListPresentationDto
		{
			NukiSmartLocksList =
			{
				new KerberoSmartLockPresentationDto<NukiSmartLockState>()
				{
					ExternalSmartLockId = 0,
					ExternalAccountId = 0,
					ExternalType = 0,
					ExternalName = "kquarter",
					ExternalState = new NukiSmartLockState
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
			}
		});
	}	
	
	[Fact]
	public async Task GetNukiSmartLocksList_UnauthorizedRequest_Test()
	{
		// Arrange
		_nukiClient.Setup(c => c.GetNukiSmartLockList(It.IsAny<int>()))
			.Returns(async () => await Task.FromResult(Result.Fail(new UnauthorizedAccessError())));
		// Act
		var nukiSmartLocksList = await _interactor.Handle(new NukiAuthenticatedRequestDto());

		// Assert
		nukiSmartLocksList.IsFailed.Should().BeTrue();
		nukiSmartLocksList.Errors.First().Should().BeEquivalentTo(new UnauthorizedAccessError());
	}
}