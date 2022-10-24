using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Interactors;
using Kerbero.Domain.NukiActions.Models.ExternalRequests;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiActions.Interactors;

public class CreateNukiSmartLockInteractorTest
{
    private readonly CreateNukiSmartLockInteractor _interactor;
    private readonly Mock<INukiSmartLockExternalRepository> _smartLockClient;
    private readonly Mock<INukiSmartLockPersistentRepository> _smartLockPersistent;

    private readonly Task<Result<NukiSmartLockExternalResponse>> _clientResponse = Task.FromResult(Result.Ok(new NukiSmartLockExternalResponse()
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
    }));

    public CreateNukiSmartLockInteractorTest()
    {
        _smartLockClient = new Mock<INukiSmartLockExternalRepository>();
        _smartLockPersistent = new Mock<INukiSmartLockPersistentRepository>();
        _interactor = new CreateNukiSmartLockInteractor(_smartLockClient.Object, _smartLockPersistent.Object);
    }

    [Fact]
    public async Task Handle_Success_Test()
    {
        // Arrange
        _smartLockClient.Setup(c => c.GetNukiSmartLock(It.IsAny<NukiSmartLockExternalRequest>()))
            .Returns(_clientResponse);
        _smartLockPersistent.Setup(c => c.CreateSmartLock(It.IsAny<NukiSmartLock>()))
            .Returns(Task.FromResult(Result.Ok(new NukiSmartLock
            {
                Favourite = true,
                Name = "kquarter",
                Type = 0,
                AuthId = 0,
                ExternalSmartLockId = 0,
                NukiAccountId = 0,
                State = new NukiSmartLockState
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
            })));
        
        // Act
        var result = await _interactor.Handle(new CreateNukiSmartLockPresentationRequest("ACCESS_TOKEN", 0));
        
        // Assert
        _smartLockClient.Verify(c =>
            c.GetNukiSmartLock(It.Is<NukiSmartLockExternalRequest>(req =>
                req.ExternalId == 0 && req.AccessToken == "ACCESS_TOKEN")));
        _smartLockPersistent.Verify(c => c.CreateSmartLock(It.Is<NukiSmartLock>(req => req.Favourite == true &&
                                                                req.Name == "kquarter" && 
                                                                req.Type == 0 &&
                                                                req.NukiAccountId == 0 &&
                                                                req.AuthId == 0 &&
                                                                req.ExternalSmartLockId == 0 &&
                                                                req.State != null)));        

        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_ExternalFailure_Test()
    {
        // Arrange
        _smartLockClient.Setup(c => c.GetNukiSmartLock(It.IsAny<NukiSmartLockExternalRequest>()))
            .Returns(async () => await Task.FromResult(Result.Fail(new UnauthorizedAccessError())));
        
        // Act
        var result = await _interactor.Handle(new CreateNukiSmartLockPresentationRequest("ACCESS_TOKEN", 0));
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Should().BeEquivalentTo(new UnauthorizedAccessError());

    }

    [Fact]
    public async Task Handle_PersistentFailure_Test()
    {
        // Arrange
        _smartLockClient.Setup(c => c.GetNukiSmartLock(It.IsAny<NukiSmartLockExternalRequest>()))
            .Returns(_clientResponse);
        _smartLockPersistent.Setup(c => c.CreateSmartLock(It.IsAny<NukiSmartLock>()))
            .Returns(async () => await Task.FromResult(Result.Fail(new PersistentResourceNotAvailableError())));
        
        // Act
        var result = await _interactor.Handle(new CreateNukiSmartLockPresentationRequest("ACCESS_TOKEN", 0));
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Should().BeEquivalentTo(new PersistentResourceNotAvailableError());

    }
}