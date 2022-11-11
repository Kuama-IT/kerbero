using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Errors;
using Kerbero.Domain.NukiActions.Interactors;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Models.ExternalRequests;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.Interactors.NukiActions;

public class OpenNukiSmartLockInteractorTests
{
    private readonly OpenNukiSmartLockInteractor _interactor;
    private readonly Mock<INukiSmartLockPersistentRepository> _persistent;
    private readonly Mock<INukiSmartLockExternalRepository> _external;

    public OpenNukiSmartLockInteractorTests()
    {
        _persistent = new Mock<INukiSmartLockPersistentRepository>();
        _external = new Mock<INukiSmartLockExternalRepository>();
        _interactor = new OpenNukiSmartLockInteractor(_persistent.Object, _external.Object);
        _interactor.Should().BeAssignableTo<IOpenNukiSmartLockInteractor>();
    }

    [Fact]
    public async Task Handle_Success_Test()
    {
        // Arrange
        _persistent.Setup(c => c.GetById(It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new NukiSmartLock
            {
                ExternalSmartLockId = 0
            })));
        _external.Setup(c => c.OpenNukiSmartLock(It.IsAny<NukiSmartLockExternalRequest>()))
            .Returns(Task.FromResult(Result.Ok()));
        
        // Act
        var result = await _interactor.Handle(new OpenNukiSmartLockPresentationRequest("ACCESS_TOKEN", 0)); // get external id from db, then call the client
        
        // Assert
        _persistent.Verify(c => c.GetById(It.Is<int>(x => x == 0)));
        _external.Verify(c => c.OpenNukiSmartLock(
                It.Is<NukiSmartLockExternalRequest>(z => z.ExternalId == 0)));
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_SmartLockNotFoundError_Test()
    {
        // Arrange
        _persistent.Setup(c => c.GetById(It.IsAny<int>()))
            .Returns(async () => await Task.FromResult(Result.Fail(new SmartLockNotFoundError())));
        
        // Act
        var result = await _interactor.Handle(new OpenNukiSmartLockPresentationRequest("ACCESS_TOKEN", 0 )); // get external id from db, then call the client
        
        // Assert
        _persistent.Verify(c => c.GetById(It.Is<int>(x => x == 0)));
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Should().BeEquivalentTo(new SmartLockNotFoundError());
        result.Errors.First().Message.Should().Be("The id provided is not associated to any SmartLock.");
    }
    
    [Fact]
    public async Task Handle_OpenIsNotPossible_Test()
    {
        // Arrange
        _persistent.Setup(c => c.GetById(It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new NukiSmartLock
            {
                ExternalSmartLockId = 0
            })));
        _external.Setup(c => c.OpenNukiSmartLock(It.IsAny<NukiSmartLockExternalRequest>()))
            .Returns(async () => await Task.FromResult(Result.Fail(new ExternalServiceUnreachableError("It is not possible open the SmartLock due to an external error."))));
        
        // Act
        var result = await _interactor.Handle(new OpenNukiSmartLockPresentationRequest("ACCESS_TOKEN", 0)); // get external id from db, then call the client

        // Assert
        _persistent.Verify(c => c.GetById(It.Is<int>(x => x == 0)));
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Should().BeOfType<ExternalServiceUnreachableError>();
        result.Errors.First().Message.Should().Be("It is not possible open the SmartLock due to an external error.");
    }
    
}